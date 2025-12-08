namespace MagpieTest.Services.IPC;

using System;
using System.Numerics;
using Magpie.Services.IPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class VnavClientTest
{
    [TestMethod]
    public void DefersToVnavIpcForPathfindAndMoveCloseTo()
    {
        var called = false;
        Func<Vector3, bool, float, bool> method = (_, _, _) => called = true;

        var ipc = new Mock<VnavIpc>();
        ipc.Object.PathfindAndMoveCloseTo = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.IsTrue(vnav.PathfindAndMoveCloseTo(new Vector3(), true, 1.0f));
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToVnavIpcForPathfindInProgress()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<VnavIpc>();
        ipc.Object.PathfindInProgress = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.IsTrue(vnav.PathfindInProgress());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToVnavIpcForIsRunning()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<VnavIpc>();
        ipc.Object.IsRunning = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.IsTrue(vnav.IsRunning());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToVnavIpcForBuildProgress()
    {
        const float expected = 1.234f;
        var called = false;
        Func<float> method = () => {
            called = true;
            return expected;
        };

        var ipc = new Mock<VnavIpc>();
        ipc.Object.BuildProgress = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.AreEqual(expected, vnav.BuildProgress());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToVnavIpcForReload()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<VnavIpc>();
        ipc.Object.Reload = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.IsTrue(vnav.Reload());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToVnavIpcForRebuild()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<VnavIpc>();
        ipc.Object.Rebuild = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.IsTrue(vnav.Rebuild());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToVnavIpcForIsReady()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<VnavIpc>();
        ipc.Object.IsReady = method;

        var vnav = new VnavClient(ipc.Object);
        Assert.IsTrue(vnav.IsReady());
        Assert.IsTrue(called);
    }
}
