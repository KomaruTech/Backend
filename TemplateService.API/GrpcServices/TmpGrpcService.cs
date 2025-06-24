using Grpc.Core;
using TemplateService.Protobuf;

namespace TemplateService.API.GrpcServices;

public class TmpGrpcService : TemplateGrpcService.TemplateGrpcServiceBase
{
    public override Task<TestMethodResponse> TestMethod(TestMethodRequest request, ServerCallContext context)
    {
        return Task.FromResult(new TestMethodResponse
        {
            Success = true,
            Message = $"RequestId: {request.RequestId} Message: {request.Message}"
        });
    }
}
