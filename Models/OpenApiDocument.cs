using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sam.OpenApi.Models
{
    public sealed class OpenApiDocument
    {
        public string OpenApi { get; set; }
        public OpenApiInfo Info { get; set; }
        public List<OpenApiServer> Servers { get; set; }
        public Dictionary<string, OpenApiPathItem> Paths { get; set; }
        public OpenApiComponents Components { get; set; }
        public List<OpenApiSecurityRequirement> Security { get; set; }
        public List<OpenApiTag> Tags { get; set; }
        public OpenApiExternalDocumentation ExternalDocs { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
    }

    public sealed class OpenApiInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TermsOfService { get; set; }
        public OpenApiContact Contact { get; set; }
        public OpenApiLicense License { get; set; }
        public string Version { get; set; }
    }

    public sealed class OpenApiContact
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
    }

    public sealed class OpenApiLicense
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public sealed class OpenApiExternalDocumentation
    {
        public string Description { get; set; }
        public string Url { get; set; }
    }

    public sealed class OpenApiTag
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public OpenApiExternalDocumentation ExternalDocs { get; set; }
    }

    public sealed class OpenApiServer
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public Dictionary<string, OpenApiServerVariable> Variables { get; set; }
    }

    public sealed class OpenApiServerVariable
    {
        public List<string> Enum { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
    }

    public sealed class OpenApiPathItem
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public Dictionary<string, OpenApiOperation> Operations { get; set; }
        public List<OpenApiServer> Servers { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public sealed class OpenApiOperation
    {
        public List<string> Tags { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public OpenApiExternalDocumentation ExternalDocs { get; set; }
        public string OperationId { get; set; }
        public List<Dictionary<string, object>> Parameters { get; set; }
        public OpenApiRequestBody RequestBody { get; set; }
        public Dictionary<string, OpenApiResponse> Responses { get; set; }
        public Dictionary<string, object> Callbacks { get; set; }
        public bool Deprecated { get; set; }
        public List<OpenApiSecurityRequirement> Security { get; set; }
        public List<OpenApiServer> Servers { get; set; }
    }

    public sealed class OpenApiRequestBody
    {
        public string Description { get; set; }
        public Dictionary<string, OpenApiMediaType> Content { get; set; }
        public bool Required { get; set; }
    }

    public sealed class OpenApiMediaType
    {
        public object Schema { get; set; }
        public object Example { get; set; }
        public Dictionary<string, object> Examples { get; set; }
        public Dictionary<string, OpenApiEncoding> Encoding { get; set; }
    }

    public sealed class OpenApiEncoding
    {
        public string ContentType { get; set; }
        public Dictionary<string, OpenApiHeader> Headers { get; set; }
        public string Style { get; set; }
        public bool Explode { get; set; }
        public bool AllowReserved { get; set; }
    }

    public sealed class OpenApiHeader
    {
        public string Description { get; set; }
        public object Schema { get; set; }
        public object Example { get; set; }
        public Dictionary<string, object> Examples { get; set; }
    }

    public sealed class OpenApiResponse
    {
        public string Description { get; set; }
        public Dictionary<string, OpenApiHeader> Headers { get; set; }
        public Dictionary<string, OpenApiMediaType> Content { get; set; }
        public Dictionary<string, object> Links { get; set; }
    }

    public sealed class OpenApiComponents
    {
        public Dictionary<string, object> Schemas { get; set; }
        public Dictionary<string, object> Responses { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Dictionary<string, object> Examples { get; set; }
        public Dictionary<string, object> RequestBodies { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public Dictionary<string, object> SecuritySchemes { get; set; }
        public Dictionary<string, object> Links { get; set; }
        public Dictionary<string, object> Callbacks { get; set; }
    }

    public sealed class OpenApiSecurityRequirement : Dictionary<string, List<string>> { }
}
