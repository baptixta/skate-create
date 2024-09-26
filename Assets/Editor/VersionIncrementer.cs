using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;

public class VersionIncrementer : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    private static VersionIncrement incrementType = VersionIncrement.Revision;

    private enum VersionIncrement
    {
        Major,
        Minor,
        Build,
        Revision
    }

    [MenuItem("Version/Increment Major", false, 1)]
    private static void SetIncrementMajor() { incrementType = VersionIncrement.Major; }

    [MenuItem("Version/Increment Minor", false, 2)]
    private static void SetIncrementMinor() { incrementType = VersionIncrement.Minor; }

    [MenuItem("Version/Increment Build", false, 3)]
    private static void SetIncrementBuild() { incrementType = VersionIncrement.Build; }

    [MenuItem("Version/Increment Revision", false, 4)]
    private static void SetIncrementRevision() { incrementType = VersionIncrement.Revision; }

    [MenuItem("Version/Show Current Increment", false, 50)]
    private static void ShowCurrentIncrement()
    {
        EditorUtility.DisplayDialog("Current Increment", $"Current increment type: {incrementType}", "OK");
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        string currentVersion = PlayerSettings.bundleVersion;
        Version version = new Version(currentVersion);

        int major = version.Major;
        int minor = version.Minor;
        int build = version.Build;
        int revision = version.Revision;

        switch (incrementType)
        {
            case VersionIncrement.Major:
                major++;
                minor = 0;
                build = 0;
                revision = 0;
                break;
            case VersionIncrement.Minor:
                minor++;
                build = 0;
                revision = 0;
                break;
            case VersionIncrement.Build:
                build++;
                revision = 0;
                break;
            case VersionIncrement.Revision:
                revision++;
                break;
        }

        Version newVersion = new Version(major, minor, build, revision);
        PlayerSettings.bundleVersion = newVersion.ToString();

        Debug.Log($"Incremented {incrementType} version to: {PlayerSettings.bundleVersion}");
    }
}
