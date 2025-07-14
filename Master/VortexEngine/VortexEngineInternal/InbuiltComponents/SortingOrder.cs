namespace VortexEngine;

public class SortingOrder : Component
{

    public int sortingOrder;

    public override void Update()
    {
        body.SortingOrder = sortingOrder;
    }

}
