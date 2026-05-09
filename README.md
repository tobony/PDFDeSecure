[![Build status](https://ci.appveyor.com/api/projects/status/qoan7ibgh7vcd74r?svg=true)](https://ci.appveyor.com/project/abatsakidis/pdfdesecure)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/16a61ca3fda34415849d93e1f79e731d)](https://www.codacy.com/gh/abatsakidis/PDFDeSecure/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=abatsakidis/PDFDeSecure&amp;utm_campaign=Badge_Grade)

[![Stargazers repo roster for @abatsakidis/PDFDeSecure](https://reporoster.com/stars/dark/abatsakidis/PDFDeSecure)](https://github.com/abatsakidis/PDFDeSecure/stargazers)

## Description ##

An easy-to-use PDF Unlocker. Remove copy-protection from PDF files. 

## How To ##

* Select your PDF Protected File (Browse).
* Click 'Unlock' button and Save the Un-Protected PDF File. 

![Alt text](/Screenshot/screen.jpg?raw=true "MD5 Bruter")

<br>

## Tested on ##

**OS**: Windows 10 x86_64 <br>
**CPU**: Intel 2 Quad Q6600 (4) @ 2.400GHz <br>
**Memory**: 4085MiB <br>

## Build ##

* Use Visual Studio 2017+<br>
* Open application's solution file (PDFDeSecure.sln)<br>

### CLI build (Windows)

```powershell
nuget restore PDFDeSecure.sln
msbuild PDFDeSecure.sln /t:Build /p:Configuration=Release
```

### If `msbuild` is not found

`PDFDeSecure` is a .NET Framework WinForms app, so Linux/macOS shells usually do not include MSBuild by default.
Use one of these test methods:

1. **GitHub Actions (recommended in non-Windows dev environments)**
   * Push branch and let `.github/workflows/build-windows.yml` run on `windows-latest`.
2. **Local Windows machine / Visual Studio Developer Command Prompt**
   * Run `nuget restore` then `msbuild` commands above.
3. **AppVeyor / other Windows CI**
   * Keep the same two commands in the CI pipeline.

## Author ##

Batsakidis Athanasios<br>
a.batsakidis@re-think.gr
