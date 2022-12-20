using NUnit.Framework;
using DIContainer.Configuration;
using DIContainer.Container;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSimpleCase()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.DoesNotThrow(() => config.Register<Service1, Service1Impl>(), "Can't register di");
            DependencyContainer di = new DependencyContainer(config);
            Service1Impl obj;
            Assert.DoesNotThrow(() => obj = (Service1Impl)di.Resolve<Service1>(), "Can't create instance");
        }


        [Test]
        public void TestSingelton()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.DoesNotThrow(() => config.Register<Service1, Service1Impl>(IConfiguration.LifePeriod.SINGELTON), "Can't register di");
            DependencyContainer di = new DependencyContainer(config);
            Service1Impl obj1 = null, obj2 = null;
            Assert.DoesNotThrow(() => obj1 = (Service1Impl)di.Resolve<Service1>(), "Can't create instance");
            Assert.DoesNotThrow(() => obj2 = (Service1Impl)di.Resolve<Service1>(), "Can't create instance");
            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.IsTrue(obj1 == obj2, "Doesn't singeltons");
        }

        [Test]
        public void TestPerInstance()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.DoesNotThrow(() => config.Register<Service1, Service1Impl>(), "Can't register di");
            DependencyContainer di = new DependencyContainer(config);
            Service1Impl obj1 = null, obj2 = null;
            Assert.DoesNotThrow(() => obj1 = (Service1Impl)di.Resolve<Service1>(), "Can't create instance");
            Assert.DoesNotThrow(() => obj2 = (Service1Impl)di.Resolve<Service1>(), "Can't create instance");
            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.IsFalse(obj1 == obj2, "Was singeltons");
        }


        [Test]
        public void GetNotRegisteredType()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            DependencyContainer di = new DependencyContainer(config);
            Service1Impl obj;
            Assert.Catch(() => obj = (Service1Impl)di.Resolve<Service1>(), "Bad creation");
        }


        [Test]
        public void TryRegisterNotCompatibleTypes()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.Catch(() => config.Register<Service1, Service1BadImpl>(), "Bad registration");
        }


        [Test]
        public void TryRegisterValueType()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.Catch(() => config.Register<int, Service1BadImpl>(), "Bad registration");
        }


        [Test]
        public void TryRegisterAbstractType()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.Catch(() => config.Register<int, Service1AbstractImpl>(), "Bad registration");
        }


        [Test]
        public void RegisterTypeAfter()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            DependencyContainer di = new DependencyContainer(config);
            Assert.DoesNotThrow(() => config.Register<Service1, Service1Impl>(), "Can't register di");
            Service1Impl obj;
            Assert.Catch(() => obj = (Service1Impl)di.Resolve<Service1>(), "Can't create instance");
        }


        [Test]
        public void RegisterCloseGenericType()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.DoesNotThrow(() => config.Register<IGenericService<int>, IGenericServiceImpl<int>>(), "Bad registration");
            DependencyContainer di = new DependencyContainer(config);
            IGenericServiceImpl<int> obj;
            Assert.DoesNotThrow(() => obj = (IGenericServiceImpl<int>)di.Resolve<IGenericService<int>>(), "Can't create instance");
        }


        [Test]
        public void RegisterOpenGenericType()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.DoesNotThrow(() => config.Register(typeof(IGenericService<>), typeof(IGenericServiceImpl<>)), "Bad registration");
            DependencyContainer di = new DependencyContainer(config);
            IGenericServiceImpl<int> obj;
            Assert.DoesNotThrow(() => obj = (IGenericServiceImpl<int>)di.Resolve<IGenericService<int>>(), "Can't create instance");
        }


        [Test]
        public void RegisterOpenGenericWithNonGenericType()
        {
            DependenciesCongifuration config = new DependenciesCongifuration();
            Assert.DoesNotThrow(() => config.Register(typeof(IGenericService<>), typeof(Service1Impl)), "Bad registration");
            DependencyContainer di = new DependencyContainer(config);
            IGenericServiceImpl<int> obj;
            Assert.Catch(() => obj = (IGenericServiceImpl<int>)di.Resolve<IGenericService<int>>(), "Can't create instance");
        }
    }
}