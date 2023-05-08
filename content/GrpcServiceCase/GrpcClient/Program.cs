// See https://aka.ms/new-console-template for more information
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using static GrpcServer.Greeter;

Console.WriteLine("Hello, World!");
string address = "https://localhost:7239";
var grpcChannel = GrpcChannel.ForAddress(address);
var greetClient = new GreeterClient(grpcChannel);

Console.WriteLine("------------------------------下面开始调用--------------------------------------");
var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZ3JwYyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiZ3JwYyIsImV4cCI6MTY4MzUyNDUwNywiaXNzIjoiR3JlZXRlciIsImF1ZCI6IkdyZWV0ZXIifQ.mUNe83jK3A-GDZi6psa3GG6sqiffovQ8XOBvWtUKK2o";

var headers = new Metadata
{
    { "Authorization", $"Bearer {token}" }
};

var sayHiResponse = await greetClient.SayHiAsync(new HelloRequest { Name = "hi" }, headers);
Console.WriteLine($"响应Hi消息为：{sayHiResponse.Message}");

var sayHelloResponse = await greetClient.SayHelloAsync(new HelloRequest { Name = "hello" }, headers);
Console.WriteLine($"响应Hello消息为：{sayHelloResponse.Message}");

Console.ReadLine();