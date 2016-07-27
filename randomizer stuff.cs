Random gen = new Random();


class RND
{
    private Random rnd;

    /// <summary>
    /// Initializes a new instance of the <see cref="RND"/> class.
    /// </summary>
    public RND()
    {
        rnd = new Random();
    }

    /// <summary>
    /// Gets the random boolean.
    /// </summary>
    /// <returns></returns>
    public bool GetRandomBoolean()
    {
        return rnd.Next(0, 2) == 0;
    }
}


public static bool GetRandomBoolean()
{
  return new Random().Next(100) % 2 == 0;
}
