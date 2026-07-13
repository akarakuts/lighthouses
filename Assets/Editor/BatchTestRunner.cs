#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace LighthouseMatch3.Editor
{
    public static class BatchTestRunner
    {
        public static void RunEditModeTests()
        {
            Run(TestMode.EditMode, "LighthouseMatch3.EditModeTests", "Release/editmode-tests.xml");
        }

        private static void Run(TestMode mode, string assemblyName, string resultPath)
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new TestCallbacks(resultPath));
            api.Execute(new ExecutionSettings(new Filter
            {
                testMode = mode,
                assemblyNames = new[] { assemblyName }
            }));
        }

        private sealed class TestCallbacks : ICallbacks
        {
            private readonly string _resultPath;

            public TestCallbacks(string resultPath)
            {
                _resultPath = resultPath;
            }

            public void RunStarted(ITestAdaptor testsToRun) { }

            public void RunFinished(ITestResultAdaptor result)
            {
                Directory.CreateDirectory("Release");
                File.WriteAllText(_resultPath, result.ToXml().OuterXml);
                Debug.Log($"Tests finished: {result.PassCount} passed, {result.FailCount} failed.");
                EditorApplication.Exit(result.FailCount == 0 ? 0 : 1);
            }

            public void TestStarted(ITestAdaptor test) { }

            public void TestFinished(ITestResultAdaptor result) { }
        }
    }
}
#endif
