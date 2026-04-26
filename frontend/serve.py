"""Tiny static file server for the CaptureCore frontend.

Adds HTTP Range support to Python's stdlib SimpleHTTPRequestHandler so that
<video> elements can seek (required for the scroll-driven video on the home
page). Plus permissive CORS for local dev.

Usage:  python serve.py [port]
"""

import os
import sys
from http.server import HTTPServer, SimpleHTTPRequestHandler


class RangeHandler(SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Accept-Ranges", "bytes")
        self.send_header("Cache-Control", "no-store")
        super().end_headers()

    def do_GET(self):
        path = self.translate_path(self.path)
        if not os.path.isfile(path):
            return super().do_GET()

        size = os.path.getsize(path)
        rng = self.headers.get("Range")
        if not rng:
            return super().do_GET()
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

        with open(path, "rb") as f:
            f.seek(start)
            remaining = length
            chunk = 64 * 1024
            while remaining > 0:
                data = f.read(min(chunk, remaining))
                if not data:
                    break
                try:
                    self.wfile.write(data)
                except (ConnectionAbortedError, ConnectionResetError, BrokenPipeError):
                    return
                remaining -= len(data)


if __name__ == "__main__":
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 5500
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    print(f"CaptureCore frontend on http://localhost:{port}/")
    HTTPServer(("127.0.0.1", port), RangeHandler).serve_forever()
