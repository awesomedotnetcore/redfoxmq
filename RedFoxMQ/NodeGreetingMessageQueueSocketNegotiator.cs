﻿// 
// Copyright 2013-2014 Hans Wolff
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

using RedFoxMQ.Transports;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedFoxMQ
{
    class NodeGreetingMessageQueueSocketNegotiator : INodeGreetingMessageNegotiator
    {
        private readonly IQueueSocket _queueSocket;

        public NodeGreetingMessageQueueSocketNegotiator(IQueueSocket queueSocket)
        {
            if (queueSocket == null) throw new ArgumentNullException("queueSocket");
            _queueSocket = queueSocket;
        }

        public void WriteGreeting(NodeGreetingMessage greetingMessage)
        {
            var messageFrame = new MessageFrame { RawMessage = greetingMessage.Serialize() };
            _queueSocket.Write(messageFrame);
        }

        public async Task WriteGreetingAsync(NodeGreetingMessage greetingMessage, CancellationToken cancellationToken)
        {
            WriteGreeting(greetingMessage);
        }

        public void VerifyRemoteGreeting(NodeType expectedNodeType)
        {
            var remoteGreeting = ReadGreeting();
            if (remoteGreeting.NodeType != expectedNodeType)
                throw new RedFoxProtocolException(
                    String.Format("Remote greeting node type was {0} but expected node type is {1}", remoteGreeting.NodeType, expectedNodeType));
        }

        private NodeGreetingMessage ReadGreeting()
        {
            var messageFrame = _queueSocket.Read();
            return NodeGreetingMessage.DeserializeWithoutLength(messageFrame.RawMessage, 1);
        }

        public async Task VerifyRemoteGreetingAsync(NodeType expectedNodeType, CancellationToken cancellationToken)
        {
            var remoteGreeting = await ReadGreetingAsync(cancellationToken);
            if (remoteGreeting.NodeType != expectedNodeType)
                throw new RedFoxProtocolException(
                    String.Format("Remote greeting node type was {0} but expected node type is {1}", remoteGreeting.NodeType, expectedNodeType));
        }

        private async Task<NodeGreetingMessage> ReadGreetingAsync(CancellationToken cancellationToken)
        {
            return ReadGreeting();
        }
    }
}
