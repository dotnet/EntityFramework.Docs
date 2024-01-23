﻿using System.Threading.Tasks;
using Cosmos.ModelBuilding;

namespace Cosmos;

public class Program
{
    static async Task Main()
    {
        await Sample.Run();
        await UnstructuredData.Sample.Run();
    }
}