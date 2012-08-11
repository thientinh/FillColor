using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ToMau
{
    class Context
    {
        static Context instance = null;
        public static Context getInstance()
        {
            if (instance == null)
            {
                instance = new Context();
            }
            return instance;
        }
        public Bitmap bitmap;
        public Image img;
        public Color colorStart;
        public Color colorFill;
        public Color colorBoundary;
        public int x;
        public int y;

        IStrategy strategy ;
        IStrategy strategy1, strategy2;
        public Context()
        {
            strategy1 = new QueueLinearFloodFill();
            strategy2 = new StackLinearFloodFill();
            strategy = strategy1;
        }
        public int AlgorithmFill(int _x, int _y)
        {
            x = _x;
            y = _y;
            return strategy.Fill(this);
        }
        public void SwitchStrategy()
        {
            if (strategy is QueueLinearFloodFill)
            {
                strategy = strategy2;
            }
            else
            {
                strategy = strategy1;
            }
        }
       
    }
}
