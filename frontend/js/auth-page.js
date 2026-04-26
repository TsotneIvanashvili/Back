

(function () {
  const tabLogin    = document.getElementById("tab-login");
  const tabRegister = document.getElementById("tab-register");
  const loginForm    = document.getElementById("login-form");
  const registerForm = document.getElementById("register-form");
  const loginError    = document.getElementById("login-error");
  const registerError = document.getElementById("register-error");
  const mode = new URLSearchParams(location.search).get("mode");
  if (mode === "register") show("register");

  tabLogin.addEventListener("click",    () => show("login"));
  tabRegister.addEventListener("click", () => show("register"));

  function show(which) {
    if (which === "register") {
      tabRegister.classList.add("active");
      tabLogin.classList.remove("active");
      registerForm.classList.remove("hidden");
      loginForm.classList.add("hidden");
    } else {
      tabLogin.classList.add("active");
      tabRegister.classList.remove("active");
      loginForm.classList.remove("hidden");
      registerForm.classList.add("hidden");
    }
    loginError.innerHTML = "";
    registerError.innerHTML = "";
  }

  function nextDest() {
    const next = new URLSearchParams(location.search).get("next");
    return next || "index.html";
  }

  function err(target, message) {
    target.innerHTML = `<div class="error-msg">${message}</div>`;
  }

  loginForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    loginError.innerHTML = "";
    const fd = new FormData(loginForm);
    try {
      const res = await API.post("/auth/login", {
        usernameOrEmail: fd.get("usernameOrEmail"),
        password: fd.get("password")
      });
      Auth.saveSession(res);
      location.href = nextDest();
    } catch (e2) {
      err(loginError, e2.message || "Could not sign in.");
    }
  });

  registerForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    registerError.innerHTML = "";
    const fd = new FormData(registerForm);
    const payload = {};
    fd.forEach((v, k) => (payload[k] = v));
    try {
      const res = await API.post("/auth/register", payload);
      Auth.saveSession(res);
      location.href = nextDest();
    } catch (e2) {
      let msg = e2.message || "Could not create account.";
      if (e2.payload && e2.payload.errors) {
        msg = Object.values(e2.payload.errors).flat().join(" ");
      }
      err(registerError, msg);
    }
  });
})();
