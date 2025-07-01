using BackendGeems.Application;
using BackendGeems.Domain;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public void EliminarEmpleador_BorradoLogico_SiEmpresaTienePagos()
        {
            int cedula = 1;
            string cedulaEmpresa = "EMP123";

            _empresaRepoMock.Setup(r => r.ObtenerEmpresaDueno(cedula)).Returns(cedulaEmpresa);
            _empresaRepoMock.Setup(r => r.ContarPagosEmpresa(cedulaEmpresa)).Returns(10);
            _empresaRepoMock.Setup(r => r.BorrarEmpleadorLogico(cedula));
            _empresaRepoMock.Setup(r => r.GetEmpleados(cedulaEmpresa)).Returns(new List<Empleado>());
            _empresaRepoMock.Setup(r => r.GetEmpresa(cedulaEmpresa)).Returns(new Empresa { CedulaJuridica = cedulaEmpresa });
            _empresaRepoMock.Setup(r => r.BorradoFisico(cedulaEmpresa));
            _beneficioRepoMock.Setup(r => r.GetCompanyBenefits(cedulaEmpresa)).Returns(new List<object>());
            _beneficioRepoMock.Setup(r => r.EliminarBeneficio(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarLogicoEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarPermanenteEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.UsuarioEstaBorrado(It.IsAny<string>())).Returns(false);
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpleadoPorCedula(It.IsAny<string>())).Returns("Empleado Test");
            _empleadoRepoMock.Setup(r => r.ObtenerCorreoEmpleadoPorCedula(It.IsAny<string>())).Returns("empleado@test.com");
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpresaPorCedula(It.IsAny<string>())).Returns("Empresa Test");
            _empleadoRepoMock.Setup(r => r.VerificarRelacionEmpleadoPlanilla(It.IsAny<string>())).Returns(true);
            _empleadoRepoMock.Setup(r => r.BorrarTimesheetEmpleado(It.IsAny<string>(), It.IsAny<bool>()));
            _reporteServiceMock.Setup(r => r.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _queryEmpresa.EliminarEmpleador(cedula);

            _empresaRepoMock.Verify(r => r.BorrarEmpleadorLogico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorrarEmpleadorFisico(cedula), Times.Never);
        }

        [Test]
        public void EliminarEmpleador_BorradoFisico_SiEmpresaNoTienePagos()
        {
            int cedula = 2;
            string cedulaEmpresa = "EMP456";

            _empresaRepoMock.Setup(r => r.ObtenerEmpresaDueno(cedula)).Returns(cedulaEmpresa);
            _empresaRepoMock.Setup(r => r.ContarPagosEmpresa(cedulaEmpresa)).Returns(0);
            _empresaRepoMock.Setup(r => r.BorrarEmpleadorFisico(cedula));
            _empresaRepoMock.Setup(r => r.GetEmpleados(cedulaEmpresa)).Returns(new List<Empleado>());
            _empresaRepoMock.Setup(r => r.GetEmpresa(cedulaEmpresa)).Returns(new Empresa { CedulaJuridica = cedulaEmpresa });
            _empresaRepoMock.Setup(r => r.BorradoFisico(cedulaEmpresa));
            _beneficioRepoMock.Setup(r => r.GetCompanyBenefits(cedulaEmpresa)).Returns(new List<object>());
            _beneficioRepoMock.Setup(r => r.EliminarBeneficio(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarLogicoEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.BorrarPermanenteEmpleado(It.IsAny<string>()));
            _empleadoRepoMock.Setup(r => r.UsuarioEstaBorrado(It.IsAny<string>())).Returns(false);
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpleadoPorCedula(It.IsAny<string>())).Returns("Empleado Test");
            _empleadoRepoMock.Setup(r => r.ObtenerCorreoEmpleadoPorCedula(It.IsAny<string>())).Returns("empleado@test.com");
            _empleadoRepoMock.Setup(r => r.ObtenerNombreEmpresaPorCedula(It.IsAny<string>())).Returns("Empresa Test");
            _empleadoRepoMock.Setup(r => r.VerificarRelacionEmpleadoPlanilla(It.IsAny<string>())).Returns(false);
            _empleadoRepoMock.Setup(r => r.BorrarTimesheetEmpleado(It.IsAny<string>(), It.IsAny<bool>()));
            _reporteServiceMock.Setup(r => r.EnviarCorreoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _queryEmpresa.EliminarEmpleador(cedula);

            _empresaRepoMock.Verify(r => r.BorrarEmpleadorFisico(cedula), Times.Once);
            _empresaRepoMock.Verify(r => r.BorrarEmpleadorLogico(cedula), Times.Never);
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
