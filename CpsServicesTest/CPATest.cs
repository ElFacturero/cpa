using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CpsServicesTest
{
    [TestClass]
    public class CPATest
    {

        private string errorAltura = @"Disculpe, puede que el nombre de la calle esté mal escrito o la altura sea incorrecta o la calle pertenezca a una localidad vecina. Por favor verifique los datos ingresados e intente nuevamente.";

        protected string ErrorAltura
        {
            get { return errorAltura; }
            set { errorAltura = value; }
        }
        /*
        [TestMethod]
        public void Test9Julio()
        {
            var svc = new CPA.CPA();
            var cpa= svc.GetCPAByNombre("S", "Rosario", "9 de julio", "2151");

            Assert.IsTrue(cpa == "S2000BOQ");
        }

        [TestMethod]
        public void TestPciaInvalida()
        {
            var svc = new CPA.CPA();
            var cpa = svc.GetCPAByNombre("SX", "Rosario", "9 de julio", "2151");

            Assert.IsNull(cpa);
        }

        [TestMethod]
        public void TestAlturaInvalida()
        {
            var svc = new CPA.CPA();
            var cpa = svc.GetCPAByNombre("S", "Rosario", "julio", "-2151");

            Assert.AreEqual(cpa, errorAltura);
        }

        [TestMethod]
        public void TestCalleInvalida()
        {
            var svc = new CPA.CPA();
            var cpa = svc.GetCPAByNombre("S", "Rosario", "z", "2151");

            Assert.IsNull(cpa);
        }

        [TestMethod]
        public void TestLocInvalida()
        {
            var svc = new CPA.CPA();
            var cpa = svc.GetCPAByNombre("S", "sad", "9 de julio", "2151");

            Assert.IsNull(cpa);
        }


        [TestMethod]
        public void TestNull()
        {
            var svc = new CPA.CPA();
            var cpa = svc.GetCPAByNombre(null, null,null, null);

            Assert.IsNull(cpa );
        }
        */
    }
}
