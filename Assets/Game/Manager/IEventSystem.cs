using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Manager
{
    public interface ITaskEventSystem
    {
        void Start();

        void Update();

        void Dispose();

    }


    public interface ITaskEventSystem<T1>:ITaskEventSystem
    {
        void Start(T1 o);


    }
    public interface ITaskEventSystem<T1,T2>:ITaskEventSystem
    {
        void Start(T1 t1, T2 t2);


    }
    public interface ITaskEventSystem<T1,T2,T3>:ITaskEventSystem
    {
        void Start(T1 t1, T2 t2, T3 t3);


    }
}