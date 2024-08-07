using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace Generics.Tables;

public class Table<T1, T2, T3>
{
    public List<T1> Rows {  get; private set; }
    public List<T2> Columns { get; private set; }
    public NestedIndexer Open;
    public NestedIndexer Existed;
    private readonly Dictionary<T1, Dictionary<T2, T3>> values = new ();

    public Table()
    {
        this.Rows = new List<T1>();
        this.Columns = new List<T2>();
        this.Open = new NestedIndexer("Open", this.Rows, this.Columns, this.values);
        this.Existed = new NestedIndexer("Existed", this.Rows, this.Columns, this.values);
    }

    public void AddRow(T1 row)
    {
        AddItem<T1>(this.Rows, row);
    }
    public void AddColumn(T2 column)
    {
        AddItem<T2>(this.Columns, column);
    }
    private void AddItem<T>(List<T> collection, T item)
    {
        if(!collection.Contains(item))
        {
            collection.Add(item);
        }
    }

    public class NestedIndexer
    {
        private string indexerType;
        private List<T1> rows;
        private List<T2> columns;
        private Dictionary<T1, Dictionary<T2, T3>> values;

        internal NestedIndexer(string indexerType, List<T1> rows, List<T2> cols, Dictionary<T1, Dictionary<T2, T3>> values)
        {
            this.indexerType = indexerType;
            this.rows = rows;
            this.columns = cols;
            this.values = values;
        }

        private bool RowAndColumnExists(T1 row, T2 column)
        {
            return rows.Contains(row) && columns.Contains(column);
        }

        private void ValidateIndexer()
        {
            if (!(this.indexerType.Equals("Open")
                || this.indexerType.Equals("Existed")))
            {
                throw new ArgumentException("Invalid indexer");
            }
        }

        private T3 GetValue(T1 row, T2 column)
        {
            if (this.values.ContainsKey(row))
            {
                return this.values[row][column];
            }
            else
            {
                return default;
            }
        }

        private void AddValue(T1 row, T2 column, T3 value)
        {
            this.values.Add(row, new Dictionary<T2, T3> { { column, value } });
        }

        public T3 this[T1 row, T2 column]
        {
            get
            {
                ValidateIndexer();

                if (this.RowAndColumnExists(row, column))
                {
                    return this.GetValue(row, column);
                }
                else if (this.indexerType.Equals("Open"))
                {
                    return default;
                }
                else
                {
                    throw new ArgumentException("Row or column doesn`t exists");
                }
            }
            set
            {
                ValidateIndexer();

                if (this.RowAndColumnExists(row, column))
                {
                    this.AddValue(row, column, value);
                }
                else if(this.indexerType.Equals("Open"))
                {
                    this.AddValue(row, column, value);
                    this.columns.Add(column);
                    this.rows.Add(row);
                }
                else
                {
                    throw new ArgumentException("Invalid row or column");
                }
            }
        }
    }
}