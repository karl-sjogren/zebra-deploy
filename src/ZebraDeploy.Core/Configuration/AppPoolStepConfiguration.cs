﻿using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class AppPoolStepConfiguration : StripeStepConfiguration {
        public string Name { get; private set; }
        public string Action { get; private set; }
        
        internal AppPoolStepConfiguration(XElement element) {
            Name = element.Attribute("name").ValueOrDefault();
            Action = element.Name == "startAppPool" ? "start" : "stop";
        }
    }
}
