"""Tiny static file server for the CaptureCore frontend.

Adds HTTP Range support to Python's stdlib SimpleHTTPRequestHandler so that
<video> elements can seek (required for the scroll-driven video on the home
page). Plus permissive CORS for local dev.

Uses ThreadingHTTPServer + per-request error swallowing so that the browser
aborting a connection (which it does often during scroll-driven video
seeking) never crashes the server.

Usage:  python serve.py [port]
"""

import os
import sys
from http.server import ThreadingHTTPServer, SimpleHTTPRequestHandler


# Errors the browser routinely produces when it cancels an in-flight request
# (e.g. scroll-driven video seeks, navigating away mid-load). We swallow these
# so the worker thread doesn't die.
ABORT_ERRORS = (
    ConnectionAbortedError,
    ConnectionResetError,
    BrokenPipeError,
    TimeoutError,
)


class RangeHandler(SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Accept-Ranges", "bytes")
        self.send_header("Cache-Control", "no-store")
        super().end_headers()

    def handle_one_request(self):
        # Wrap the entire request lifecycle so an aborted connection doesn't
        # propagate up and kill the thread / spam the console.
        try:
            super().handle_one_request()
        except ABORT_ERRORS:
            self.close_connection = True

    def copyfile(self, source, outputfile):
        # Default copy uses shutil.copyfileobj, which raises on closed sockets.
        # Override so we silently exit when the client goes away.
        try:
            super().copyfile(source, outputfile)
        except ABORT_ERRORS:
            pass

    def do_GET(self):
        path = self.translate_path(self.path)
        if not os.path.isfile(path):
            return super().do_GET()

        size = os.path.getsize(path)
        rng = self.headers.get("Range")
        if not rng:
            return super().do_GET()

        # parse "bytes=START-END" (END is optional)
        try:
            unit, _, spec = rng.partition("=")
            if unit.strip().lower() != "bytes":
                raise ValueError
            start_s, _, end_s = spec.partition("-")
            start = int(start_s) if start_s else 0
            end = int(end_s) if end_s else size - 1
        except ValueError:
            self.send_error(416, "Invalid Range")
            return
        if start >= size or end >= size or start > end:
            self.send_error(416, "Range Not Satisfiable")
            return

        length = end - start + 1
        ctype = self.guess_type(path)
        self.send_response(206, "Partial Content")
        self.send_header("Content-Type", ctype)
        self.send_header("Content-Length", str(length))
        self.send_header("Content-Range", f"bytes {start}-{end}/{size}")
        self.end_headers()

        try:
            with open(path, "rb") as f:
                f.seek(start)
                remaining = length
                chunk = 64 * 1024
                while remaining > 0:
                    data = f.read(min(chunk, remaining))
                    if not data:
                        break
                    self.wfile.write(data)
                    remaining -= len(data)
        except ABORT_ERRORS:
            return


if __name__ == "__main__":
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 5500
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    print(f"CaptureCore frontend on http://localhost:{port}/")
    # Threaded so a slow client / crashed connection doesn't block the rest.
    ThreadingHTTPServer(("127.0.0.1", port), RangeHandler).serve_forever()
