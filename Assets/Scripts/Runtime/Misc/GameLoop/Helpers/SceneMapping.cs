using System.Collections.Generic;
using UnityEngine;

public static class SceneMapping
{
    private static readonly Dictionary<Scene, string> sceneNameMap = new Dictionary<Scene, string>()
    {
        { Scene.MainMenu, "MainMenu" },
        { Scene.Game, "Game" }
    };

    public static string GetSceneName(Scene scene)
    {
        if (sceneNameMap.TryGetValue(scene, out string name))
        {
            return name;
        }
        else
        {
            Debug.LogError($"Scene name for {scene} not found.");
            return null;
        }
    }

    public enum Scene
    {
        MainMenu,
        Game
    }
}
