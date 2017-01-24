using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition(ResponseType = typeof(StartSimulationMessageResponse))]
    public struct StartSimulationMessageRequest
    {
    }

    [MessageDefinition(IsResponse = true)]
    public struct StartSimulationMessageResponse
    {
        public bool Result;

        public StartSimulationMessageResponse(bool result)
        {
            Result = result;
        }
    }
}