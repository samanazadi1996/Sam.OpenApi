# OpenApi Generator

A simple tool to generate source code from an OpenAPI Specification (Swagger).
You can use this tool to generate client code (services, interfaces, etc.) from a Swagger file or URL.
Currently, it supports Angular, and more languages will be added in the future.

## Usage
``` bash
OpenApi <type> (<url> | <file>) <outputPath>
```

## Examples
- Using a URL:
``` bash
OpenApi angular "https://example.com/swagger/v1/swagger.json" "C:\output"
```
- Using a local file:
``` bash
OpenApi angular "C:\api.json" "C:\output"
```
## Arguments

- type → The output type (e.g., angular; more languages coming soon)
- url | file → The Swagger source (can be a valid URL or a local file path)
- outputPath → The output directory where files will be generated

## Output
- Depending on the selected type, the generated files will vary:
  - For Angular:
    - Angular services for API communication
    - TypeScript interfaces for models
  - Other languages/frameworks will be added in the future
 

## Roadmap

Planned language and framework support:

Language / Framework | Status 
--- | --- 
Angular | ✅ Ready
React | ⏳ Planned
C# (Client SDK) | ⏳ Planned
Others | ⏳ Planned
