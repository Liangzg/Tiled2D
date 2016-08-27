
public interface ITiledTrigger
{
    void OnTiledEnter(ITiledTrigger tiled);

    void OnTiledStay(ITiledTrigger tiled);

    void OnTiledExit(ITiledTrigger tiled);
}
