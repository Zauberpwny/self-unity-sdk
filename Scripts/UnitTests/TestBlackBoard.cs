﻿/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Collections;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.Self.Topics;
using IBM.Watson.Self.Agents;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.Self.UnitTests
{
    public class TestBlackBoard : UnitTest
    {
        string m_TargetPath = "";
            
        bool m_bSubscribeTested = false;
        bool m_ConnectionClosed = false;

        TopicClient m_Client = null;
        BlackBoard m_BlackBoard = null;

        public override IEnumerator RunTest()
        {
            m_Client = new TopicClient();
            m_BlackBoard = new BlackBoard(m_Client);

            if ( m_Client.IsActive )
            {
                m_Client.Disconnect();
                while( m_Client.IsActive ) 
                    yield return null;
            }

            m_Client.StateChangedEvent += OnStateChanged;

            m_Client.Connect();

            while(! m_bSubscribeTested )
                yield return null;

            Log.Debug( "TestBlackBoard", "Tested Subscription now disconnecting" );
            m_Client.Disconnect();
                
            while(! m_ConnectionClosed )
                yield return null;

            m_Client.StateChangedEvent -= OnStateChanged;

            yield break;
        }

        void OnStateChanged(TopicClient.ClientState a_CurrentState)
        {
            Log.Debug( "TestBlackBoard", "OnStateChanged to {0}" , a_CurrentState);

            switch (a_CurrentState)
            {
                case TopicClient.ClientState.Connected:
                    OnConnected();
                    break;
                case TopicClient.ClientState.Disconnected:
                    OnDisconnected();
                    break;
                default:
                    break;
            }
        }

        private void OnConnected()
        {
            Log.Debug( "TestBlackBoard", "OnConnected" );
            m_BlackBoard.SubscribeToType( "Text", OnText, a_Path:m_TargetPath );
            m_ConnectionClosed = false;
        }

        private void OnDisconnected()
        {
            Log.Debug( "TestBlackBoard", "OnDisconnected" );
            m_ConnectionClosed = true;
        }

        private void OnText( ThingEvent a_Event )
        {
            Log.Debug( "TestBlackBoard", "OnText : {0}", a_Event );
            m_bSubscribeTested = true;

        }
    }
}

