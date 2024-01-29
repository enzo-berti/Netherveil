using System.Collections;

public abstract class Sbire : Mobs
{
    private void Start()
    {
        StartCoroutine(Brain());
    }

    protected abstract IEnumerator Brain();
}