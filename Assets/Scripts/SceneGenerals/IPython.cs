using SaveAndLoad;
using System;
using System.IO;
using UnityEngine;

public class IPython : MonoBehaviour
{
    public MemoryStream stream;
    public VRIDEWriter writer;

    public Microsoft.Scripting.Hosting.ScriptEngine pythonEngine;
    public Microsoft.Scripting.Hosting.ScriptScope pythonScope;

    void Start()
    {   
        pythonEngine = IronPython.Hosting.Python.CreateEngine();

        var paths = new[] {
            Path.Combine(Application.persistentDataPath),
            Path.Combine(Application.streamingAssetsPath),
            Path.Combine(Application.streamingAssetsPath, "Lib"),
            Path.Combine(Application.streamingAssetsPath, "Lib", "site-packages"),
            Path.Combine(Application.persistentDataPath, SaveAndLoadModule.username)
        };

        pythonEngine.SetSearchPaths(paths);

        pythonScope = pythonEngine.CreateScope();

        InitializeStream();
    }

    public void Execute(string code)
    {
        try 
        {
            pythonEngine.Execute(code, pythonScope);
            writer.Write("\n\nProgram executed successfully.");
        }
        catch (Exception e)
        {
            writer.Write("<color=#C63737>[Error] " + e.Message + "\n" + e.StackTrace + "</color>");
            writer.Write("\n\nProgram failed execution.");
        }
    }

    void InitializeStream()
    {
        stream = new MemoryStream();
        writer = new VRIDEWriter(stream);

        pythonEngine.Runtime.IO.SetOutput(stream, writer);
        pythonEngine.Runtime.IO.SetErrorOutput(stream, writer);
    }

    public void ResetStream()
    {
        if (stream != null) stream.Dispose();
        if (writer != null) writer.Dispose();
        InitializeStream();
    }
}
