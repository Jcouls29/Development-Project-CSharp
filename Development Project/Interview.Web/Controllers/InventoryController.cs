//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.7.0.0 (NJsonSchema v10.1.24.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using Domain.Models;

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."

namespace Interview.Web.Controllers
{
    using System = global::System;
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.7.0.0 (NJsonSchema v10.1.24.0 (Newtonsoft.Json v12.0.0.0))")]
    public interface IController
    {
        /// <summary>Get an inventory item on metadata</summary>
        /// <returns>The find results.</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Product>> FindProductOnMetadataAsync(MetaData body);
    
        /// <summary>Add a product on metadata</summary>
        /// <returns>The find results.</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Product>> AddProductAsync();
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.7.0.0 (NJsonSchema v10.1.24.0 (Newtonsoft.Json v12.0.0.0))")]
    public partial class Controller : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private IController _implementation;
    
        public Controller(IController implementation)
        {
            _implementation = implementation;
        }
    
        /// <summary>Get an inventory item on metadata</summary>
        /// <returns>The find results.</returns>
        [Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("search")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Product>> FindProductOnMetadata([Microsoft.AspNetCore.Mvc.FromBody] MetaData body)
        {
            return _implementation.FindProductOnMetadataAsync(body);
        }
    
        /// <summary>Add a product on metadata</summary>
        /// <returns>The find results.</returns>
        [Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("add")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Product>> AddProduct()
        {
            return _implementation.AddProductAsync();
        }
    
    }

    

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108