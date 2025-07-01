using BackendGeems.Application;
using BackendGeems.Domain;
using Moq;
using NUnit.Framework;
using System;

namespace UnitTests
{
    public class EliminarEmpleadorTest
    {
        private Mock<IEmpresaRepo> _empresaRepoMock;
        private Mock<IPagoRepo> _pagoRepoMock;
        private Mock<IBeneficioRepo> _beneficioRepoMock;
        private Mock<IEmpleadoRepo> _empleadoRepoMock;
        private Mock<IReporteService> _reporteServiceMock;
        private QueryEmpresa _queryEmpresa;

        [SetUp]
        public void Setup()
        {
            _empresaRepoMock = new Mock<IEmpresaRepo>();
            _pagoRepoMock = new Mock<IPagoRepo>();
            _beneficioRepoMock = new Mock<IBeneficioRepo>();
            _empleadoRepoMock = new Mock<IEmpleadoRepo>();
            _reporteServiceMock = new Mock<IReporteService>();

            _queryEmpresa = new QueryEmpresa(
                _empresaRepoMock.Object,
                _pagoRepoMock.Object,
                _beneficioRepoMock.Object,
                _empleadoRepoMock.Object,
                _reporteServiceMock.Object
            );
        }

        [Test]
        public void EliminarEmpleador_SinPagos_RealizaBorradoFisico()
        {
            int cedula = 1;
            string cedulaEmpresa = "EMP123";
            _empresaRepoMock.Setup(r => r.ObtenerEmpresaDueno(cedula)).Returns(cedulaEmpresa);
            _empresaRepoMock.Setup(r => r.ContarPagosEmpresa(cedulaEmpresa)).Returns(0);
            _empresaRepoMock.Setup(r => r.BorrarEmpleadorFisico(cedula));
            _empresaRepoMock.Setup(r => r.BorradoFisico(cedulaEmpresa));

            _queryEmpresa.EliminarEmpleador(cedula);

            _empresaRepoMock.Verify(r => r.BorrarEmpleadorFisico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorrarEmpleadorLogico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpleador_ConPagos_RealizaBorradoLogico()
        {
            int cedula = 1;
            string cedulaEmpresa = "EMP123";
            _empresaRepoMock.Setup(r => r.ObtenerEmpresaDueno(cedula)).Returns(cedulaEmpresa);
            _empresaRepoMock.Setup(r => r.ContarPagosEmpresa(cedulaEmpresa)).Returns(5);
            _empresaRepoMock.Setup(r => r.BorrarEmpleadorLogico(cedula));
            _empresaRepoMock.Setup(r => r.BorradoFisico(cedulaEmpresa));

            _queryEmpresa.EliminarEmpleador(cedula);

            _empresaRepoMock.Verify(r => r.BorrarEmpleadorLogico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorrarEmpleadorFisico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpleador_ThrowsException_PropagaExcepcion()
        {
            int cedula = 1;
            _empresaRepoMock.Setup(r => r.ObtenerEmpresaDueno(cedula)).Throws(new Exception("Error"));

            Assert.Throws<ArgumentException>(() => _queryEmpresa.EliminarEmpleador(cedula));
        }
    }
}
