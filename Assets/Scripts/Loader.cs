using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{
    // IMPORTANT NOTE:
    // Bir sahne olu�turup build listesine verdi�imizde
    // Birebir ayn� ismini buraya eklememiz gerekmekte!
    public enum Scene
    {
        GameScene,
        LoadingScene,
        MainMenuScene,
        CharacterSelectScene
    }

    public static int targetSceneIndex;

    private static Scene _targetScene;

    // Herkeste ayn� anda y�klenecek sahneler haricinde
    // Oyuncunun tek ba��na ge�ti�i b�t�n sahne ge�i�lerini bu fosksiyon ile yap�yoruz.
    // Oyuncuyu bo� bir "Loading" sahnesine al�yor ve o sahnede hedefledi�imiz sahneyi y�klemeye ba�l�yor
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
