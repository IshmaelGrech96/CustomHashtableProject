// See https://aka.ms/new-console-template for more information

using CustomHashTableProject;
using UniversalHashFunctionProvider;

CustomHashTable<int, string> customHashTable = new CustomHashTable<int, string>();

IHashFunctionProvider provider = new CarterWegmanHashFunctionProvider();

int tableSize = 31;
IHashFunction hashFunction = provider.GetHashFunction(tableSize);

var sc = new CustomHashTableThreadSafe<int,string>();
var tasks = new List<Task>();
int itemsWritten = 0;

tasks.Add(Task.Run(() => {
    String[] vegetables = { "broccoli", "cauliflower",
                            "carrot", "sorrel", "baby turnip",
                            "beet", "brussel sprout",
                            "cabbage", "plantain",
                            "spinach", "grape leaves",
                            "lime leaves", "corn",
                            "radish", "cucumber",
                            "raddichio", "lima beans" };
    for (int ctr = 1; ctr <= vegetables.Length; ctr++)
        sc.Insert(ctr, vegetables[ctr - 1]);

    itemsWritten = vegetables.Length;
    Console.WriteLine("Task {0} wrote {1} items\n",
                      Task.CurrentId, itemsWritten);
}));

// Execute two readers, one to read from first to last and the second from last to first.
for (int ctr = 0; ctr <= 1; ctr++)
{
    bool desc = ctr == 1;
    tasks.Add(Task.Run(() => {
        int start, last, step;
        int items;
        do
        {
            String output = String.Empty;
            items = sc.Size;
            if (!desc)
            {
                start = 1;
                step = 1;
                last = items;
            }
            else
            {
                start = items;
                step = -1;
                last = 1;
            }

            for (int index = start; desc ? index >= last : index <= last; index += step)
                output += String.Format("[{0}] ", sc.Search(index));

            Console.WriteLine("Task {0} read {1} items: {2}\n",
                              Task.CurrentId, items, output);
        } while (items < itemsWritten | itemsWritten == 0);
    }));
}
// Execute a red/update task.
tasks.Add(Task.Run(() => {
    Thread.Sleep(100);
    for (int ctr = 1; ctr <= sc.Size; ctr++)
    {
        String value = sc.Search(ctr);
        if (value == "cucumber")
            sc.Update(ctr, "green bean");
                
    }
}));

// Wait for all three tasks to complete.
Task.WaitAll(tasks.ToArray());

// Display the final contents of the cache.
Console.WriteLine();
Console.WriteLine("Values in synchronized cache: ");
for (int ctr = 1; ctr <= sc.Size; ctr++)
    Console.WriteLine("   {0}: {1}", ctr, sc.Search(ctr));

