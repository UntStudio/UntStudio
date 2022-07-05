﻿namespace UntStudio.Bootstrapper.Models
{
    internal class LoaderEntryPoint
    {
        public string Namespace;

        public string Class;

        public string Method;



        public LoaderEntryPoint(string @namespace, string @class, string method)
        {
            Namespace = @namespace;
            Class = @class;
            Method = method;
        }
    }
}
