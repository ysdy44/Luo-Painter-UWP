## Pixel Shader 

This folder contains some custom pixel shaders for use by PixelShaderEffect.


<br/>

## Development environment

|Key|Value|
|---|---|
|Development tool|Visual Studio 2017|
|Programing language|HLSL|
|Compile tool|Developer Command Prompt|
|Compile command|CompileShaders.cmd|
|Shader source code|*.hlsl File|
|Binaries|*.bin File|


<br/>

## 引用

无


<br/>

## Nuget 引用

无


<br/>

### Example

```csharp
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Effects;
using Windows.Storage;
using Windows.Storage.Streams;
...
public static async Task<PixelShaderEffect> CreaAsync(IGraphicsEffectSource source, float parameter1)
{
    // TODO: Please replace "MyHLSL.bin" with the File Name.
    Uri uri = new Uri("ms-appx:///Luo Painter.Shaders/MyHLSL.bin");

    // Read all bytes.
    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
    IBuffer buffer = await FileIO.ReadBufferAsync(file);
    byte[] shaderCode = buffer.ToArray();

    // Create pixel shader.
    PixelShaderEffect shader = new PixelShaderEffect(shaderCode)
    {
        Source1BorderMode = EffectBorderMode.Hard,
        Source1 = source,
        Properties =
        {
            // TODO: Please replace "Parameter1" with the HLSL Parameter.
            ["parameter1"] = parameter1,
        }
    };

    return shader;
}
...
```

## Deployment instructions

> 1. Run the Developer Command Prompt for VS2017,
> 
> 2. Input "cd CurrentFolderPath" and press enter to jump to current folder,
> 
> 3. Input "CompileShaders.cmd" and press enter to run CompileShaders.cmd.
> 
> (This will recompile *.hlsl files, generating the *.bin output binaries)
>
> 4. In the Solution View, Put the *.bin File into this Folder;
>
> 5. In the Properties View, Set the "Build Action" of the *.bin File to "Content".