﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Genguid {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.3.0")]
    internal sealed partial class Genguid : global::System.Configuration.ApplicationSettingsBase {
        
        private static Genguid defaultInstance = ((Genguid)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Genguid())));
        
        public static Genguid Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection guidFormatters {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["guidFormatters"]));
            }
            set {
                this["guidFormatters"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string guidFactory {
            get {
                return ((string)(this["guidFactory"]));
            }
            set {
                this["guidFactory"] = value;
            }
        }
    }
}
