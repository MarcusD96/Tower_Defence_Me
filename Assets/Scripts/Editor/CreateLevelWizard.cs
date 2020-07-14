using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CreateLevelWizard : ScriptableWizard {

    Scene scene = new Scene();
    static int sceneNum = 3;

    [MenuItem("TD Tools/Create New Level")]
    static void CreateWizard() {
        DisplayWizard<CreateLevelWizard>("Make a New Level", "Create");
    }

    void OnWizardCreate() {
        MakeScene();
    }

    void MakeScene() {
        sceneNum++;
        scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        scene.name = "Level" + sceneNum;
    }
}
