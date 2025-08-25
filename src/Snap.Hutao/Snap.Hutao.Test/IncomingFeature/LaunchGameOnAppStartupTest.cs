// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public sealed class LaunchGameOnAppStartupTest
{
    [TestMethod]
    public void SettingEntryConstantExists()
    {
        // Arrange & Act
        string settingKey = SettingEntry.LaunchGameOnAppStartup;

        // Assert
        Assert.IsNotNull(settingKey, "LaunchGameOnAppStartup setting constant should exist");
        Assert.AreEqual("Launch.GameOnAppStartup", settingKey, "Setting key should match expected value");
    }

    [TestMethod]
    public void SettingKeyFollowsNamingConvention()
    {
        // Arrange & Act
        string settingKey = SettingEntry.LaunchGameOnAppStartup;

        // Assert
        Assert.IsTrue(settingKey.StartsWith("Launch."), "Launch setting should start with 'Launch.' prefix");
        Assert.IsFalse(string.IsNullOrWhiteSpace(settingKey), "Setting key should not be null or whitespace");
    }
}