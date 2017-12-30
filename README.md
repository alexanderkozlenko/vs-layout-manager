## Visual Studio 2017 Setup Layout Manager

Provides an ability to display and remove obsolete packages from the Visual Studio 2017 installation layout. Each package which is not listed in the current catalog file `Catalog.json` is considered as absolete.

### Run instruction:

```
dotnet vslm.dll --layout <value> [--command <value>]
```

### Supported parameters:

Parameter | Mandatory | Default Value | Purpose
--- | :---: | --- | ---
`--layout` | Yes | | Installation layout path
`--command` | No | `reveal` | Command to execute

### Supported commands:

Command | Purpose
--- | ---
`reveal` | List obsolete packages
`clean` | Remove obsolete packages

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/vs-layout-manager.svg?style=flat-square)](https://github.com/alexanderkozlenko/vs-layout-manager/releases)