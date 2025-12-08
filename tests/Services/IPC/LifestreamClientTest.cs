namespace MagpieTest.Services.IPC;

using System;
using Magpie.Services.IPC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class LifestreamClientTest
{
    [TestMethod]
    public void DefersToLifestreamIpcForAethernetTeleport()
    {
        var called = false;
        Func<string, bool> method = (_) => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.AethernetTeleport = method;

        var client = new LifestreamClient(ipc.Object);
        Assert.IsTrue(client.AethernetTeleport("destination"));
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToLifestreamIpcForTeleport()
    {
        var called = false;
        Func<uint, byte, bool> method = (_, _) => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.Teleport = method;

        var client = new LifestreamClient(ipc.Object);
        Assert.IsTrue(client.Teleport(1u, 1));
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToLifestreamIpcForTeleportToHome()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.TeleportToHome = method;

        var client = new LifestreamClient(ipc.Object);
        Assert.IsTrue(client.TeleportToHome());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToLifestreamIpcForTeleportToFC()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.TeleportToFC = method;

        var client = new LifestreamClient(ipc.Object);
        Assert.IsTrue(client.TeleportToFC());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToLifestreamIpcForTeleportToApartment()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.TeleportToApartment = method;

        var client = new LifestreamClient(ipc.Object);
        Assert.IsTrue(client.TeleportToApartment());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToLifestreamIpcForIsBusy()
    {
        var called = false;
        Func<bool> method = () => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.IsBusy = method;

        var client = new LifestreamClient(ipc.Object);
        Assert.IsTrue(client.IsBusy());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DefersToLifestreamIpcForExecuteCommand()
    {
        var called = false;
        Action<string> method = (_) => called = true;

        var ipc = new Mock<LifestreamIpc>();
        ipc.Object.ExecuteCommand = method;

        var client = new LifestreamClient(ipc.Object);
        client.ExecuteCommand("command");
        Assert.IsTrue(called);
    }
}
