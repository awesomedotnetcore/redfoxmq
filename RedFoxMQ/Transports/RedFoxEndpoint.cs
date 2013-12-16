﻿// 
// Copyright 2013 Hans Wolff
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
using System;
using System.Collections.Generic;

namespace RedFoxMQ.Transports
{
    public struct RedFoxEndpoint : IEquatable<RedFoxEndpoint>, IEqualityComparer<RedFoxEndpoint>
    {
        public static readonly RedFoxEndpoint Empty = new RedFoxEndpoint();

        public RedFoxTransport Transport;
        public string Host;
        public int Port;
        public string Path;

        public RedFoxEndpoint(string path)
            : this(RedFoxTransport.Inproc, null, 0, path)
        {
        }

        public RedFoxEndpoint(string host, int port)
            : this(host, port, null)
        {
        }

        public RedFoxEndpoint(string host, int port, string path)
            : this(RedFoxTransport.Tcp, host, port, path)
        {
        }

        public RedFoxEndpoint(RedFoxTransport transport, string host, int port, string path)
        {
            Transport = transport;
            Host = host;
            Port = port;
            Path = path;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RedFoxEndpoint)) return false;
            return Equals(this, (RedFoxEndpoint)obj);
        }

        public bool Equals(RedFoxEndpoint other)
        {
            return Equals(this, other);
        }

        public bool Equals(RedFoxEndpoint x, RedFoxEndpoint y)
        {
            return
                x.Port == y.Port &&
                x.Host == y.Host &&
                x.Path == y.Path &&
                x.Transport == y.Transport;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public int GetHashCode(RedFoxEndpoint x)
        {
            unchecked
            {
                var hashCode = (int)Transport;
                hashCode = (hashCode * 397) ^ (Host != null ? Host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Port;
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Transport.ToString().ToLowerInvariant() + "://" + Host + ":" + Port + Path;
        }

        public static RedFoxEndpoint Parse(string endpointUri)
        {
            if (endpointUri == null) throw new ArgumentNullException("endpointUri");

            RedFoxEndpoint endpoint;
            if (!TryParse(endpointUri, out endpoint))
                throw new FormatException(String.Format("Invalid endpoint format: {0}", endpointUri));

            return endpoint;
        }

        public static bool TryParse(string endpointUri, out RedFoxEndpoint endpoint)
        {
            endpoint = Empty;
            if (endpointUri == null) 
                return false;

            Uri uri;
            if (!Uri.TryCreate(endpointUri, UriKind.Absolute, out uri))
                return false;

            var scheme = uri.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "tcp":
                    endpoint.Transport = RedFoxTransport.Tcp;
                    break;
                case "inproc":
                    endpoint.Transport = RedFoxTransport.Inproc;
                    break;
                default:
                    return false;
            }

            endpoint.Host = uri.DnsSafeHost;
            endpoint.Port = uri.Port;

            if (endpoint.Transport != RedFoxTransport.Tcp)
                endpoint.Path = uri.PathAndQuery;

            return true;
        }
    }
}
