using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public interface Service1
    {

    }

    public class Service1Impl : Service1
    {

    }

    public class Service1BadImpl
    {

    }

    public abstract class Service1AbstractImpl
    {
        public abstract void f();
    }
}
