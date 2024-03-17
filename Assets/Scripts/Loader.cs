using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{
    // IMPORTANT NOTE:
    // Bir sahne oluþturup build listesine verdiðimizde
    // Birebir ayný ismini buraya eklememiz gerekmekte!
    public enum Scene
    {
        GameScene,
        LoadingScene,
        MainMenuScene,
        CharacterSelectScene
    }

    public static int targetSceneIndex;

    private static Scene _targetScene;

    // Herkeste ayný anda yüklenecek sahneler haricinde
    // Oyuncunun tek baþýna geçtiði bütün sahne geçiþlerini bu fosksiyon ile yapýyoruz.
    // Oyuncuyu boþ bir "Loading" sahnesine alýyor ve o sahnede hedeflediðimiz sahneyi yüklemeye baþlýyor
    public static void Load(Scene targetScene)
    {
        _targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());
    }
}
