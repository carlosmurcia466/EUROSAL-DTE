using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace EURO_CAPTURA
{
    public partial class Form1 : Form
    {

        DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["data"].ToString());
        string db1, user1, pwd1, server1;
        string db2, user2, pwd2, server2;
        string ambiente;
        string NUME_DOCUMENTO,FECHA_EMISION,HORA_EMISION, REGISTRO_FISCAL, TIPO_DOCUMENTO, FACTURA_ID,TIPODTE, FACTURA_ID_ABONONC,NOTAS, STOTALG_SUMASFACT, DIRECCION_CLIENTE;
        int tipoGeneracion;


        private void Form1_Load(object sender, EventArgs e)
        {

        }
        string codigoGeneracion,numeroControl,CODIGO_GENERACIONDTE, NUMERO_CONTROLDTE;
        string HOMOLOGADO_DUI_NIT,CODIGO_TIPOIDREC, NUMERO_CEDULA_RECEPTOR, NIT_RECEPTOR, NUMERO_REGISTRO_RECEPTOR, RAZON_SOCIAL_RECEPTOR, NOMBRE_COMERCIAL_RECEPTOR;
        string CODIGO_ACTECON, EMAIL_RECEPTOR, TELEFONO_RECEPTOR, MUNICIPIO_RECEPTOR, DEPARTAMENTO_RECEPTOR, DIRECCION_RECEPTOR, CODCLIENTE, VENDEDOR, CREDITO_, DIAS_CREDITO;
        string CODIGO_ACTIVIDAD, CODIGO_DESCRIPCION;
        string CREDITO_CONTADO,TOTAL_FACTURA, VALOR_EXENTO, VALOR_GRABADO, SUMA_GRAVA_MASIVA, VALOR_NOSUJETO, VALOR_IVA, VALOR_RETE, VALOR_DESCUENTOS;
        string FECHA_REAL_CREACION_RELACIONADO, TIPO_DOCUMENTO_RELACIONADO, NDOCUMENTO_RELACIONADO;
        string CODIGO_SUCURSAL_CLIENTE;

        string casaMatriz,numeroControl_correlativo;
        List<Identificacion> identificacion = new List<Identificacion>();
        List<detalle_relacionado> dt_re=new List<detalle_relacionado>();
        List<Emisor> emisor = new List<Emisor>();
        List<Receptor> receptor= new List<Receptor>();
        List<detalle> detalles = new List<detalle>();
        List<detalle_extension> detalle_extension= new List<detalle_extension>();
        List<Resumen> resumen= new List<Resumen>();
        List<detalle_apendice> detalle_apendice = new List<detalle_apendice>();


        List<string[]> detalle_factura= new List<string[]>();


        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tiempo.Enabled=false;
            try
            {
                foreach (var i in di.GetFiles())
                {
                    Encoding encoding = Encoding.UTF7;
                    StreamReader r = new StreamReader(i.FullName, encoding);
                    string jsonString = r.ReadToEnd();

                    var m = JsonConvert.DeserializeObject<List<principal>>(jsonString);

                    foreach (var cx in m)
                    {

                        server1 = cx.conexiones[0].servidor.ToString();
                        user1 = cx.conexiones[0].usuario.ToString();
                        pwd1 = cx.conexiones[0].pwd.ToString();
                        db1 = cx.conexiones[0].basededatos.ToString();

                        server2 = cx.conexiones[1].servidor.ToString();
                        user2 = cx.conexiones[1].usuario.ToString();
                        pwd2 = cx.conexiones[1].pwd.ToString();
                        db2 = cx.conexiones[1].basededatos.ToString();

                        ambiente = cx.conexiones[2].ambiente.ToString();


                    }





                    Data();


                }
            }
            catch
            {

            }

            tiempo.Enabled = true;
        }
        public void Data()
        {

            
            SqlConnection cxdb1 = new SqlConnection("Server=" + server1 + ";DataBase=" + db1 + ";User ID=" + user1 + ";Password=" + pwd1);
            SqlConnection cxdb2 = new SqlConnection("Server=" + server2 + ";DataBase=" + db2 + ";User ID=" + user2 + ";Password=" + pwd2);

            cxdb1.Open();

            try
            {
                string queryDocumentos = "SELECT NOMBRE_ESTABLECIMIENTO,CODIGO_SUCURSAL_CLIENTE,DIRECCION_CLIENTE,NOTAS,CODIGO_GENERACIONDTE,NUMERO_CONTROLDTE,FACTURA_ID_ABONONC,VENDEDOR,CREDITO_CONTADO,DIAS_CREDITO,FACTURA_ID,NUMERO_FACTURA,REGISTRO_FISCAL,COD_EMPRE,FECHA_EMISION,FECHA_REAL_CREACION,TIPO_DOCUMENTO,NUME_DOCUMENTO,ESTA_IMPRESA from FACTURAS where ESTA_IMPRESA is null and  TOTAL_FACTURA !='0.00' and TIPO_DOCUMENTO IN ('1','2','3','4','6') and FECHA_EMISION >='2024-04-01'";
                SqlCommand cmdDocumentos = new SqlCommand(queryDocumentos, cxdb1);

                //TABLA DOCUMENTOS

                using (SqlDataReader reader = cmdDocumentos.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        FACTURA_ID = Convert.ToString(reader["FACTURA_ID"]);
                        FACTURA_ID_ABONONC = Convert.ToString(reader["FACTURA_ID_ABONONC"]);
                        NUME_DOCUMENTO = Convert.ToString(reader["NUME_DOCUMENTO"]);
                        REGISTRO_FISCAL = Convert.ToString(reader["REGISTRO_FISCAL"]);
                        TIPO_DOCUMENTO = Convert.ToString(reader["TIPO_DOCUMENTO"]);
                        VENDEDOR = Convert.ToString(reader["VENDEDOR"]);
                        CREDITO_ = Convert.ToString(reader["CREDITO_CONTADO"]);
                        DIAS_CREDITO = Convert.ToString(reader["DIAS_CREDITO"]);
                        CODIGO_GENERACIONDTE = Convert.ToString(reader["CODIGO_GENERACIONDTE"]);
                        NUMERO_CONTROLDTE = Convert.ToString(reader["NUMERO_CONTROLDTE"]);
                        NOTAS = Convert.ToString(reader["NOTAS"]);
                        DIRECCION_CLIENTE = Convert.ToString(reader["DIRECCION_CLIENTE"]);
                        NOMBRE_COMERCIAL_RECEPTOR =  Convert.ToString(reader["NOMBRE_ESTABLECIMIENTO"]);
                        CODIGO_SUCURSAL_CLIENTE = Convert.ToString(reader["CODIGO_SUCURSAL_CLIENTE"]);
                        DateTime fechaActual = DateTime.Now;
                        string fechaFormateada = fechaActual.ToString("yyyy-MM-dd");


                        FECHA_EMISION = fechaFormateada;

                        // Obtener la hora actual
                        DateTime currentTime = DateTime.Now;

                        // Formato de hora, minutos y segundos: HH:mm:ss
                        string formattedTime = currentTime.ToString("HH:mm:ss");

                        HORA_EMISION = formattedTime;

                    }
                }
            }
            catch
            {
               
            }


            if (FACTURA_ID != null && FACTURA_ID!="")
            {

                //TABLA DOCUMENTOS RELACIONADOS  PARA NOTAS DE CREDITO

                if (TIPO_DOCUMENTO.ToString().Trim() == "3" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    string queryDocRelacionados = "SELECT CODIGO_GENERACIONDTE,NUMERO_FACTURA,SERIE_AUTORIZADA,FACTURA_ID_ABONONC,FECHA_REAL_CREACION,TIPO_DOCUMENTO from FACTURAS where FACTURA_ID='" + FACTURA_ID_ABONONC + "'";

                    SqlCommand cmdRelacionado = new SqlCommand(queryDocRelacionados, cxdb1);

                    using (SqlDataReader reader = cmdRelacionado.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                          string  NUMERO_FACTURA = Convert.ToString(reader["NUMERO_FACTURA"]).ToString().Trim();
                          string  SERIE_AUTORIZADA = Convert.ToString(reader["SERIE_AUTORIZADA"]).ToString().Trim();

                            string[] commaSeparator = new string[] { " " };
                            string[] result2;
                            FECHA_REAL_CREACION_RELACIONADO = Convert.ToString(reader["FECHA_REAL_CREACION"]).ToString().Trim();
                            result2 = FECHA_REAL_CREACION_RELACIONADO.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                            string resultado = result2[0];

                            string[] separador = new string[] { "/" };
                            string[] resultadoseparador;
                            resultadoseparador = resultado.Split(separador, StringSplitOptions.RemoveEmptyEntries);


                          FECHA_REAL_CREACION_RELACIONADO = resultadoseparador[2].Substring(0, 4).Trim() + "-" + resultadoseparador[1].Trim().PadLeft(2, '0') + "-" +resultadoseparador[0].Trim().PadLeft(2, '0');
                          TIPO_DOCUMENTO_RELACIONADO = Convert.ToString(reader["TIPO_DOCUMENTO"]).ToString().Trim();

                            if (SERIE_AUTORIZADA.Trim()== "DTE-03-SUC01")
                            {
                                NDOCUMENTO_RELACIONADO = Convert.ToString(reader["CODIGO_GENERACIONDTE"]).ToString().Trim();
                                tipoGeneracion = 2;
                            }
                            else
                            {
                                NDOCUMENTO_RELACIONADO = SERIE_AUTORIZADA + " " + NUMERO_FACTURA.PadLeft(8, '0');
                                tipoGeneracion = 1;
                            }
                            
                          


                           

                        }
                    }
                }

                //TABLA DOCUMENTOS RELACIONADOS  PARA NOTA DE DEBITO


                if (TIPO_DOCUMENTO.ToString().Trim() == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    string queryDocRelacionados = "SELECT CODIGO_GENERACIONDTE,NUMERO_FACTURA,SERIE_AUTORIZADA,FACTURA_ID_ABONONC,FECHA_REAL_CREACION,TIPO_DOCUMENTO from FACTURAS where FACTURA_ID='" + FACTURA_ID_ABONONC + "'";

                    SqlCommand cmdRelacionado = new SqlCommand(queryDocRelacionados, cxdb1);

                    using (SqlDataReader reader = cmdRelacionado.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string NUMERO_FACTURA = Convert.ToString(reader["NUMERO_FACTURA"]).ToString().Trim();
                            string SERIE_AUTORIZADA = Convert.ToString(reader["SERIE_AUTORIZADA"]).ToString().Trim();

                            string[] commaSeparator = new string[] { " " };
                            string[] result2;
                            FECHA_REAL_CREACION_RELACIONADO = Convert.ToString(reader["FECHA_REAL_CREACION"]).ToString().Trim();
                            result2 = FECHA_REAL_CREACION_RELACIONADO.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                            string resultado = result2[0];

                            string[] separador = new string[] { "/" };
                            string[] resultadoseparador;
                            resultadoseparador = resultado.Split(separador, StringSplitOptions.RemoveEmptyEntries);


                            FECHA_REAL_CREACION_RELACIONADO = resultadoseparador[2].Substring(0, 4).Trim() + "-" + resultadoseparador[1].Trim().PadLeft(2, '0') + "-" + resultadoseparador[0].Trim().PadLeft(2, '0');
                            TIPO_DOCUMENTO_RELACIONADO = Convert.ToString(reader["TIPO_DOCUMENTO"]).ToString().Trim();
                            
                            if (SERIE_AUTORIZADA.Trim() == "DTE-03-SUC01")
                            {
                                NDOCUMENTO_RELACIONADO = Convert.ToString(reader["CODIGO_GENERACIONDTE"]).ToString().Trim();
                                tipoGeneracion = 2;
                            }
                            else
                            {
                                NDOCUMENTO_RELACIONADO = SERIE_AUTORIZADA + " " + NUMERO_FACTURA.PadLeft(8, '0');
                                tipoGeneracion = 1;
                            }


                        }
                    }
                }


                //TABLA RECEPTOR
                string queryFacturas = "select TOP 1 CLIENTES.cod_clte as'CODCLIENTE',FACTURAS.NOMBRE_ESTABLECIMIENTO,FACTURAS.CODIGO_SUCURSAL_CLIENTE,CLIENTES.CODIGO_ACTECON,CLIENTES.CODIGO_TIPOIDREC,CLIENTES.HOMOLOGADO_DUI_NIT,CLIENTES.NUMERO_CEDULA,CLIENTES.NIT,CLIENTES.NUMERO_REGISTRO,CLIENTES.RAZON_SOCIAL,CLIENTES.NOMBRE_COMERCIAL,CLIENTES.EMAIL,CLIENTES.TELEFONO,SUBSTRING(MUNICIPIOS.MUNICIPIO,3,4) as MUNICIPIO,MUNICIPIOS.DEPARTAMENTO as DEPARTAMENTO, CLIENTES.DIRECCION from FACTURAS as FACTURAS,VENDEDORES as VENDEDORES,CLIENTES as CLIENTES,MUNICIPIOS as MUNICIPIOS,DEPARTAMENTOS as DEPARTAMENTOS where CLIENTES.REGISTRO_FISCAL='" + REGISTRO_FISCAL + "' and FACTURAS.VENDEDOR=VENDEDORES.VENDEDOR and FACTURAS.REGISTRO_FISCAL= CLIENTES.REGISTRO_FISCAL and CLIENTES.MUNICIPIO= MUNICIPIOS.MUNICIPIO and MUNICIPIOS.DEPARTAMENTO=DEPARTAMENTOS.DEPARTAMENTO";

                SqlCommand cmdFacturas = new SqlCommand(queryFacturas, cxdb1);

                using (SqlDataReader reader = cmdFacturas.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        NUMERO_CEDULA_RECEPTOR = Convert.ToString(reader["NUMERO_CEDULA"]);
                        NIT_RECEPTOR = Convert.ToString(reader["NIT"]).Replace("-", "").Trim();
                        NUMERO_REGISTRO_RECEPTOR = Convert.ToString(reader["NUMERO_REGISTRO"]).Replace("-", "").Trim();
                        RAZON_SOCIAL_RECEPTOR = Convert.ToString(reader["RAZON_SOCIAL"]);
                     //   NOMBRE_COMERCIAL_RECEPTOR = Convert.ToString(reader["NOMBRE_ESTABLECIMIENTO"]);
                        EMAIL_RECEPTOR = Convert.ToString(reader["EMAIL"]);
                        TELEFONO_RECEPTOR = Convert.ToString(reader["TELEFONO"]).Replace("-", "").Trim();
                        MUNICIPIO_RECEPTOR = Convert.ToString(reader["MUNICIPIO"]);
                        DEPARTAMENTO_RECEPTOR = Convert.ToString(reader["DEPARTAMENTO"]);
                        DIRECCION_RECEPTOR = Convert.ToString(reader["DIRECCION"]);
                        CODIGO_ACTECON = Convert.ToString(reader["CODIGO_ACTECON"]);
                        CODIGO_TIPOIDREC = Convert.ToString(reader["CODIGO_TIPOIDREC"]);
                        HOMOLOGADO_DUI_NIT = Convert.ToString(reader["HOMOLOGADO_DUI_NIT"]);
                        CODCLIENTE = Convert.ToString(reader["CODCLIENTE"]);
                       



                    }
                }


                //CAPTURAR CODIGO DE MUNICIPIO Y DEPARTAMENTO SI CODIGO DE SUCURSAL CLIENTE ES DIFERENTE DE VACIO

                if (CODIGO_SUCURSAL_CLIENTE.ToString().Trim() != "")
                {

                    string querycodigosucursal = "select MUNICIPIO from CLIENTES_SUCURSALES where REGISTRO_FISCAL='" + REGISTRO_FISCAL.ToString().Trim()+"' and CODIGO_SUCURSAL_CLIENTE='"+ CODIGO_SUCURSAL_CLIENTE.ToString().Trim() + "'";
                    SqlCommand cmdcodigosucursal = new SqlCommand(querycodigosucursal, cxdb1);

                    using (SqlDataReader reader = cmdcodigosucursal.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DEPARTAMENTO_RECEPTOR = Convert.ToString(reader["MUNICIPIO"]).Substring(0,2);
                            MUNICIPIO_RECEPTOR = Convert.ToString(reader["MUNICIPIO"]).Substring(Convert.ToString(reader["MUNICIPIO"]).Length - 2);



                        }
                    }



                }




                //TABLA ACTIVIDAD ECONOMICA
                if (CODIGO_ACTECON != "")
                {
                    string queryACTECON = "select TOP 1 E.CODIGO_ACTECON as CODIGO_ACTIVIDAD,E.DESCRIPCION as CODIGO_DESCRIPCION FROM CLIENTES C JOIN CAT_019ACTIVID_ECON E ON E.CODIGO_ACTECON=C.CODIGO_ACTECON  WHERE  C.CODIGO_ACTECON='" + CODIGO_ACTECON + "'";

                    SqlCommand cmdACTECON = new SqlCommand(queryACTECON, cxdb1);

                    using (SqlDataReader reader = cmdACTECON.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CODIGO_ACTIVIDAD = Convert.ToString(reader["CODIGO_ACTIVIDAD"]);
                            CODIGO_DESCRIPCION = Convert.ToString(reader["CODIGO_DESCRIPCION"]);
                        }
                    }

                }

                //TABLA FACTURAS
                string querytablafactura = "SELECT STOTALG_SUMASFACT,CREDITO_CONTADO,TOTAL_FACTURA,VALOR_DESCUENTOS,VALOR_EXENTO,VALOR_GRABADO,SUMA_GRAVA_MASIVA,VALOR_NOSUJETO,VALOR_IVA,VALOR_RETE,CANTIDAD_ABONADA,SEGURO,FLETE from FACTURAS where FACTURA_ID=" + FACTURA_ID + "";

                SqlCommand cmdtablafactura = new SqlCommand(querytablafactura, cxdb1);

                using (SqlDataReader reader = cmdtablafactura.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        TOTAL_FACTURA = Convert.ToString(reader["TOTAL_FACTURA"]);
                        VALOR_EXENTO = Convert.ToString(reader["VALOR_EXENTO"]);
                        VALOR_GRABADO = Convert.ToString(reader["VALOR_GRABADO"]);
                        VALOR_NOSUJETO = Convert.ToString(reader["VALOR_NOSUJETO"]);
                        VALOR_IVA = Convert.ToString(reader["VALOR_IVA"]);
                        VALOR_RETE = Convert.ToString(reader["VALOR_RETE"]);
                        VALOR_DESCUENTOS = Convert.ToString(reader["VALOR_DESCUENTOS"]);
                        CREDITO_CONTADO = Convert.ToString(reader["CREDITO_CONTADO"]);
                        SUMA_GRAVA_MASIVA = Convert.ToString(reader["SUMA_GRAVA_MASIVA"]);
                        STOTALG_SUMASFACT = Convert.ToString(reader["STOTALG_SUMASFACT"]);


                    }
                }


                //TABLA DETALLE FACTURAS

                string queryDetalle = "SELECT CORRELATIVO, ARTICULO,CANTIDAD_ARTICULOS,DESCRIPC_ARTICULO,PRECIO_BASE,PORC_DESCUENTO,PRECIO_UNITARIO,TIPO_VENTA,VALOR_TOTAL,VALOR_DESCUENTO,VALOR_SIN_DESCUENTO,LOTE,FECHA_VENCI,PORC_DESCUENTO,BONIFICACION_MANUAL,CANTIDAD_PAQUETES from DETALLE_FACTURAS where FACTURA_ID=" + FACTURA_ID + "";

                SqlCommand cmdDetalle = new SqlCommand(queryDetalle, cxdb1);

                using (SqlDataReader reader = cmdDetalle.ExecuteReader())
                {

                    

                    while (reader.Read())
                    {
                        string correlativo = Convert.ToString(reader["CORRELATIVO"]).Trim();
                        string articulo = Convert.ToString(reader["ARTICULO"]).Trim();
                        string cantidad = Convert.ToString(reader["CANTIDAD_ARTICULOS"]).Trim();
                        string precio_base = Convert.ToString(reader["PRECIO_BASE"]).Trim();
                        string valor_total = Convert.ToString(reader["VALOR_TOTAL"]).Trim();
                        string valor_descuento = Convert.ToString(reader["VALOR_DESCUENTO"]).Trim();
                        string lote = reader["LOTE"].ToString().Trim().Trim();
                        string fecha_venci = reader["FECHA_VENCI"].ToString().Trim();
                        string porcen_descuento = reader["PORC_DESCUENTO"].ToString().Trim();
                        string cant_paquetes = reader["CANTIDAD_PAQUETES"].ToString().Trim();
                        string descripcion_ = reader["DESCRIPC_ARTICULO"].ToString().Trim();
                    
                       if(lote==null || lote == "")
                        {
                            lote = "0";
                        }
                        if (fecha_venci == null || fecha_venci == "")
                        {
                            fecha_venci = "0";
                        }
                        if (porcen_descuento == null || porcen_descuento == "")
                        {
                            porcen_descuento = "0";
                        }
                        if (cant_paquetes == null || cant_paquetes == "")
                        {
                            cant_paquetes = "0";
                        }


                        //FACTURA_ID = 209090

                        string descripcion = Convert.ToString(descripcion_+"|"+lote+"|"+fecha_venci+"|"+porcen_descuento+"|"+cant_paquetes+"|").Trim();
                       
                        
                        
                        detalle_factura.Add(new string[] { correlativo, articulo, cantidad, descripcion.Trim(), precio_base, valor_total, valor_descuento });



                      




                    }
                }

            
               
                cxdb1.Close();


                int num =Convert.ToInt32(detalle_factura.Count());


                if (num == 1)
                {
                    if (NOTAS.ToString().Trim() != "")
                    {

                        // Obtener la longitud del párrafo
                        int longitudParrafo = NOTAS.Length;

                        // Calcular la longitud de cada parte
                        int longitudParte = longitudParrafo / 3;

                        // Dividir el párrafo en tres partes
                        string parte1 = NOTAS.Substring(0, longitudParte);
                        string parte2 = NOTAS.Substring(longitudParte, longitudParte);
                        string parte3 = NOTAS.Substring(longitudParte * 2);


                        if (parte1.ToString().Trim() != "")
                        {
                            detalle_factura.Add(new string[] { "2", "NOTAS", "1", parte1, "0", "0", "0" });
                        }

                        if (parte2.ToString().Trim() != "")
                        {
                            detalle_factura.Add(new string[] { "3", "NOTAS", "1", parte2, "0", "0", "0" });
                        }

                        if (parte3.ToString().Trim() != "")
                        {
                            detalle_factura.Add(new string[] { "4", "NOTAS", "1", parte3, "0", "0", "0" });
                        }

                    }
                }
                else if (num > 1)
                {
                    if (NOTAS.ToString().Trim() != "")
                    {

                        // Obtener la longitud del párrafo
                        int longitudParrafo = NOTAS.Length;

                        // Calcular la longitud de cada parte
                        int longitudParte = longitudParrafo / 3;

                        // Dividir el párrafo en tres partes
                        string parte1 = NOTAS.Substring(0, longitudParte);
                        int numerodeitem1 = Convert.ToInt32(num + 1);
                        string parte2 = NOTAS.Substring(longitudParte, longitudParte);
                        int numerodeitem2 = Convert.ToInt32(numerodeitem1 + 1);
                        string parte3 = NOTAS.Substring(longitudParte * 2);
                        int numerodeitem3 = Convert.ToInt32(numerodeitem2 + 1);

                        if (parte1.ToString().Trim() != "")
                        {
                            detalle_factura.Add(new string[] { Convert.ToString(numerodeitem1), "NOTAS", "1", parte1, "0", "0", "0" });
                        }

                        if (parte2.ToString().Trim() != "")
                        {
                            detalle_factura.Add(new string[] { Convert.ToString(numerodeitem2), "NOTAS", "1", parte2, "0", "0", "0" });
                        }

                        if (parte3.ToString().Trim() != "")
                        {
                            detalle_factura.Add(new string[] { Convert.ToString(numerodeitem3), "NOTAS", "1", parte3, "0", "0", "0" });
                        }

                    }


                }




                //-------------------------INICIO CREACION JSON----------------------------//
                //APARTADO PARA IDENTIFICACION

                Identificacion iden = new Identificacion();




                if (TIPO_DOCUMENTO == "2")
                {
                    iden.version = 3;
                    iden.ambiente = ambiente;
                    iden.tipoDte = "03";

                    TIPODTE = "03";

                    if (NUMERO_CONTROLDTE.ToString().Trim() != "")
                    {
                        iden.numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                        numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                    }
                    else
                    {
                        cxdb2.Open();
                        string queryccf = "SELECT casaMatriz,numeroControl_correlativo FROM ControlCorrelativos WHERE idTipoDocumento=2";
                        SqlCommand cmdCCF = new SqlCommand(queryccf, cxdb2);
                        using (SqlDataReader reader = cmdCCF.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                casaMatriz = Convert.ToString(reader["casaMatriz"]);
                                numeroControl_correlativo = Convert.ToString(reader["numeroControl_correlativo"]);
                            }
                        }


                        int correlativo = Int32.Parse(numeroControl_correlativo) + 1;
                        iden.numeroControl = "DTE-03-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');
                        numeroControl = "DTE-03-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');

                        string updateQuery = "UPDATE ControlCorrelativos SET numeroControl_correlativo = @NuevoValor WHERE idTipoDocumento = @Condicion";

                        using (SqlCommand command = new SqlCommand(updateQuery, cxdb2))
                        {

                            command.Parameters.AddWithValue("@NuevoValor", correlativo);
                            command.Parameters.AddWithValue("@Condicion", 2);
                            int rowsAffected = command.ExecuteNonQuery();

                            Console.WriteLine("Registros actualizados: " + rowsAffected);
                        }
                    }




                }
                else if (TIPO_DOCUMENTO == "1")
                {
                    iden.version = 1;
                    iden.ambiente = ambiente;
                    iden.tipoDte = "01";

                    TIPODTE = "01";

                    if (NUMERO_CONTROLDTE.ToString().Trim() != "")
                    {
                        iden.numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                        numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                    }
                    else
                    {

                        cxdb2.Open();
                        string queryccf = "SELECT casaMatriz,numeroControl_correlativo FROM ControlCorrelativos WHERE idTipoDocumento=1";
                        SqlCommand cmdCCF = new SqlCommand(queryccf, cxdb2);
                        using (SqlDataReader reader = cmdCCF.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                casaMatriz = Convert.ToString(reader["casaMatriz"]);
                                numeroControl_correlativo = Convert.ToString(reader["numeroControl_correlativo"]);
                            }
                        }


                        int correlativo = Int32.Parse(numeroControl_correlativo) + 1;
                        iden.numeroControl = "DTE-01-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');
                        numeroControl = "DTE-01-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');

                        string updateQuery = "UPDATE ControlCorrelativos SET numeroControl_correlativo = @NuevoValor WHERE idTipoDocumento = @Condicion";

                        using (SqlCommand command = new SqlCommand(updateQuery, cxdb2))
                        {

                            command.Parameters.AddWithValue("@NuevoValor", correlativo);
                            command.Parameters.AddWithValue("@Condicion", 1);
                            int rowsAffected = command.ExecuteNonQuery();

                            Console.WriteLine("Registros actualizados: " + rowsAffected);
                        }
                    }
                } 
                else if (TIPO_DOCUMENTO=="3" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {

              
                   
                        iden.version = 3;
                        iden.ambiente = ambiente;
                        iden.tipoDte = "05";

                        TIPODTE = "05";
                    if (NUMERO_CONTROLDTE.ToString().Trim() != "")
                    {
                        iden.numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                        numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                    }
                    else
                    {

                        cxdb2.Open();
                        string queryccf = "SELECT casaMatriz,numeroControl_correlativo FROM ControlCorrelativos WHERE idTipoDocumento=4";
                        SqlCommand cmdCCF = new SqlCommand(queryccf, cxdb2);
                        using (SqlDataReader reader = cmdCCF.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                casaMatriz = Convert.ToString(reader["casaMatriz"]);
                                numeroControl_correlativo = Convert.ToString(reader["numeroControl_correlativo"]);
                            }
                        }


                        int correlativo = Int32.Parse(numeroControl_correlativo) + 1;
                        iden.numeroControl = "DTE-05-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');
                        numeroControl = "DTE-05-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');

                        string updateQuery = "UPDATE ControlCorrelativos SET numeroControl_correlativo = @NuevoValor WHERE idTipoDocumento = @Condicion";

                        using (SqlCommand command = new SqlCommand(updateQuery, cxdb2))
                        {

                            command.Parameters.AddWithValue("@NuevoValor", correlativo);
                            command.Parameters.AddWithValue("@Condicion", 4);
                            int rowsAffected = command.ExecuteNonQuery();

                            Console.WriteLine("Registros actualizados: " + rowsAffected);
                        }

                    }

                }

                else if (TIPO_DOCUMENTO == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {



                    iden.version = 3;
                    iden.ambiente = ambiente;
                    iden.tipoDte = "06";

                    TIPODTE = "06";
                    if (NUMERO_CONTROLDTE.ToString().Trim() != "")
                    {
                        iden.numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                        numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                    }
                    else
                    {

                        cxdb2.Open();
                        string queryccf = "SELECT casaMatriz,numeroControl_correlativo FROM ControlCorrelativos WHERE idTipoDocumento=5";
                        SqlCommand cmdCCF = new SqlCommand(queryccf, cxdb2);
                        using (SqlDataReader reader = cmdCCF.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                casaMatriz = Convert.ToString(reader["casaMatriz"]);
                                numeroControl_correlativo = Convert.ToString(reader["numeroControl_correlativo"]);
                            }
                        }


                        int correlativo = Int32.Parse(numeroControl_correlativo) + 1;
                        iden.numeroControl = "DTE-06-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');
                        numeroControl = "DTE-06-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');

                        string updateQuery = "UPDATE ControlCorrelativos SET numeroControl_correlativo = @NuevoValor WHERE idTipoDocumento = @Condicion";

                        using (SqlCommand command = new SqlCommand(updateQuery, cxdb2))
                        {

                            command.Parameters.AddWithValue("@NuevoValor", correlativo);
                            command.Parameters.AddWithValue("@Condicion", 5);
                            int rowsAffected = command.ExecuteNonQuery();

                            Console.WriteLine("Registros actualizados: " + rowsAffected);
                        }

                    }

                }
                else if (TIPO_DOCUMENTO == "6")
                {



                    iden.version = 1;
                    iden.ambiente = ambiente;
                    iden.tipoDte = "11";

                    TIPODTE = "11";
                    if (NUMERO_CONTROLDTE.ToString().Trim() != "")
                    {
                        iden.numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                        numeroControl = NUMERO_CONTROLDTE.ToString().Trim();
                    }
                    else
                    {

                        cxdb2.Open();
                        string queryccf = "SELECT casaMatriz,numeroControl_correlativo FROM ControlCorrelativos WHERE idTipoDocumento=9";
                        SqlCommand cmdCCF = new SqlCommand(queryccf, cxdb2);
                        using (SqlDataReader reader = cmdCCF.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                casaMatriz = Convert.ToString(reader["casaMatriz"]);
                                numeroControl_correlativo = Convert.ToString(reader["numeroControl_correlativo"]);
                            }
                        }


                        int correlativo = Int32.Parse(numeroControl_correlativo) + 1;
                        iden.numeroControl = "DTE-11-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');
                        numeroControl = "DTE-11-" + casaMatriz + "-" + Convert.ToString(correlativo).PadLeft(15, '0');

                        string updateQuery = "UPDATE ControlCorrelativos SET numeroControl_correlativo = @NuevoValor WHERE idTipoDocumento = @Condicion";

                        using (SqlCommand command = new SqlCommand(updateQuery, cxdb2))
                        {

                            command.Parameters.AddWithValue("@NuevoValor", correlativo);
                            command.Parameters.AddWithValue("@Condicion", 9);
                            int rowsAffected = command.ExecuteNonQuery();

                            Console.WriteLine("Registros actualizados: " + rowsAffected);
                        }

                    }

                }








                cxdb2.Close();



                if (CODIGO_GENERACIONDTE.ToString().Trim() != "")
                {
                    codigoGeneracion = CODIGO_GENERACIONDTE;
                    iden.codigoGeneracion = codigoGeneracion;
                }
                else
                {
                    Guid g = Guid.NewGuid();
                    codigoGeneracion = g.ToString().ToUpper();
                    iden.codigoGeneracion = codigoGeneracion;
                }
                iden.tipoModelo = 1;
                iden.tipoOperacion = 1;
                iden.tipoContingencia = null;
                iden.motivoContin = null;
                iden.fecEmi = FECHA_EMISION;
                iden.horEmi = HORA_EMISION;
                iden.tipoMoneda = "USD";
                identificacion.Add(iden);


                //APARTADO PARA DOCUMENTOS RELACIONADOS

                detalle_relacionado detalle_re = new detalle_relacionado();
                List<DocumentoRelacionado> documentorelacionado = new List<DocumentoRelacionado>();


                if (TIPO_DOCUMENTO_RELACIONADO=="2" && Convert.ToInt32(FACTURA_ID_ABONONC)>0)
                {
                    
                    documentorelacionado.Add(addDocumentosRelacionados("03", tipoGeneracion, NDOCUMENTO_RELACIONADO,FECHA_REAL_CREACION_RELACIONADO));

                }








                if (documentorelacionado.Count <= 0)
                {
                    detalle_re.documentoRelacionado = null;
                }
                else
                {
                    detalle_re.documentoRelacionado = documentorelacionado;
                }


                dt_re.Add(detalle_re);





                //APARTADO PARA EMISOR
                Emisor emi = new Emisor();
                List<direccion> direccion_emisor = new List<direccion>();
                emi.nit = "06141710891014";
                emi.nrc = "43850";
                emi.nombre = "EUROSALVADOREÑA, S.A. DE C.V.";
                emi.codActividad = "46484";
                emi.descActividad = "Venta de productos farmacéuticos y medicinales";
                emi.nombreComercial = "EUROSALVADOREÑA";
                emi.tipoEstablecimiento = "01";
                emi.telefono = "22095400";
                emi.correo = "info@eurosal.com.sv";
                emi.codEstableMH = "M001";
                emi.codEstable = "M001";
                emi.codPuntoVentaMH = "P001";
                emi.codPuntoVenta = "P001";
                emi.tipoItemExpor = 1;
                emi.recintoFiscal = null;
                emi.regimen= "EX-1.1000.000";  
                

                direccion_emisor.Add(adddireccionemisor("05", "01", "Bulevar Orden de Malta, Calle El Boquerón No. 5-B, Urbanización Santa Elena"));

                emi.direccion = direccion_emisor;

                emisor.Add(emi);




                //APARTADO PARA RECEPTOR

                Receptor rec = new Receptor();
                List<direccion> direccion_receptor = new List<direccion>();

                if (TIPO_DOCUMENTO == "1")
                {

                    if (HOMOLOGADO_DUI_NIT.ToString().Trim() == "S")
                    {
                        rec.tipoDocumento = "36";
                        rec.numDocumento = NUMERO_CEDULA_RECEPTOR.Replace("-","").Trim();
                    }
                    else
                    {
                        if (NUMERO_CEDULA_RECEPTOR.ToString().Trim() == "" && NIT_RECEPTOR.ToString().Trim() == "")
                        {
                            rec.tipoDocumento = null;
                            rec.numDocumento = null;

                        }
                        else
                        {

                            if (NUMERO_CEDULA_RECEPTOR.ToString().Trim()=="")
                            {
                                rec.tipoDocumento = "36";
                                rec.numDocumento = NIT_RECEPTOR;

                            }
                            else
                            {
                                rec.tipoDocumento= "13";
                                rec.numDocumento = NUMERO_CEDULA_RECEPTOR;
                            }




                        }
                     
                      

                    }







                    if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nrc = null;
                    }
                    else
                    {
                        rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                    }

                    direccion_receptor.Add(adddireccionreceptor(DEPARTAMENTO_RECEPTOR, MUNICIPIO_RECEPTOR, DIRECCION_CLIENTE));

                    if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nrc = null;
                    }
                    else
                    {
                        rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                    }
                    if (CODIGO_ACTECON.ToString().Trim() == "")
                    {
                        rec.codActividad = "10005";
                        rec.descActividad = "OTROS";

                    }
                    else
                    {
                         rec.codActividad = CODIGO_ACTIVIDAD;
                         rec.descActividad = CODIGO_DESCRIPCION;
                    }



                    rec.nombre = RAZON_SOCIAL_RECEPTOR;
                    rec.nombreComercial = NOMBRE_COMERCIAL_RECEPTOR;
                  

                    if (TELEFONO_RECEPTOR.ToString().Trim() == "" || TELEFONO_RECEPTOR.ToString().Trim() == "0")
                    {
                        rec.telefono = null;
                    }
                    else
                    {
                        rec.telefono = TELEFONO_RECEPTOR;
                    }

                    
                    if (EMAIL_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.correo = "eurosaldte@gmail.com";
                    }
                    else
                    {
                        rec.correo = EMAIL_RECEPTOR;
                    }
                  
              

                    rec.direccion = direccion_receptor;
                }

                if (TIPO_DOCUMENTO == "2")
                {

                    if (NIT_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nit = null;
                    }
                    else
                    {
                        rec.nit = NIT_RECEPTOR;
                    }


                   
                    if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nrc = null;
                    }
                    else
                    {
                        rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                    }

                    if (CODIGO_ACTECON.ToString().Trim() == "")
                    {
                        rec.codActividad = "10005";
                        rec.descActividad = "OTROS";

                    }
                    else
                    {
                        rec.codActividad = CODIGO_ACTIVIDAD;
                        rec.descActividad = CODIGO_DESCRIPCION;
                    }


                    rec.nombre = RAZON_SOCIAL_RECEPTOR;
                    rec.nombreComercial = NOMBRE_COMERCIAL_RECEPTOR;

                    direccion_receptor.Add(adddireccionreceptor(DEPARTAMENTO_RECEPTOR, MUNICIPIO_RECEPTOR, DIRECCION_CLIENTE));

                    if (TELEFONO_RECEPTOR.ToString().Trim() == "" || TELEFONO_RECEPTOR.ToString().Trim() == "0")
                    {
                        rec.telefono = null;
                    }
                    else
                    {
                        rec.telefono = TELEFONO_RECEPTOR;
                    }

                   if (EMAIL_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.correo = "eurosaldte@gmail.com";
                    }
                    else
                    {
                        rec.correo = EMAIL_RECEPTOR;
                    }

                 
                    rec.direccion = direccion_receptor;
                }
                
                if (TIPO_DOCUMENTO == "3" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    
                    if (Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                    {

                    if (NIT_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nit = null;
                    }
                    else
                    {
                        rec.nit = NIT_RECEPTOR;
                    }



                    if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nrc = null;
                    }
                    else
                    {
                        rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                    }

                    if (CODIGO_ACTECON.ToString().Trim() == "")
                    {
                        rec.codActividad = "10005";
                        rec.descActividad = "OTROS";

                    }
                    else
                    {
                        rec.codActividad = CODIGO_ACTIVIDAD;
                        rec.descActividad = CODIGO_DESCRIPCION;
                    }


                    rec.nombre = RAZON_SOCIAL_RECEPTOR;
                    rec.nombreComercial = NOMBRE_COMERCIAL_RECEPTOR;

                    direccion_receptor.Add(adddireccionreceptor(DEPARTAMENTO_RECEPTOR, MUNICIPIO_RECEPTOR, DIRECCION_CLIENTE));

                    if (TELEFONO_RECEPTOR.ToString().Trim() == "" || TELEFONO_RECEPTOR.ToString().Trim() == "0")
                    {
                        rec.telefono = null;
                    }
                    else
                    {
                        rec.telefono = TELEFONO_RECEPTOR;
                    }

                    if (EMAIL_RECEPTOR.ToString().Trim() == "")
                     {
                            rec.correo = "eurosaldte@gmail.com";
                     }
                     else
                     {
                         rec.correo = EMAIL_RECEPTOR;
                     }

                    rec.direccion = direccion_receptor;
                        
                    }
                }

                if (TIPO_DOCUMENTO == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {

                    if (Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                    {

                        if (NIT_RECEPTOR.ToString().Trim() == "")
                        {
                            rec.nit = null;
                        }
                        else
                        {
                            rec.nit = NIT_RECEPTOR;
                        }



                        if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                        {
                            rec.nrc = null;
                        }
                        else
                        {
                            rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                        }

                        if (CODIGO_ACTECON.ToString().Trim() == "")
                        {
                            rec.codActividad = "10005";
                            rec.descActividad = "OTROS";

                        }
                        else
                        {
                            rec.codActividad = CODIGO_ACTIVIDAD;
                            rec.descActividad = CODIGO_DESCRIPCION;
                        }


                        rec.nombre = RAZON_SOCIAL_RECEPTOR;
                        rec.nombreComercial = NOMBRE_COMERCIAL_RECEPTOR;

                        direccion_receptor.Add(adddireccionreceptor(DEPARTAMENTO_RECEPTOR, MUNICIPIO_RECEPTOR, DIRECCION_CLIENTE));

                        if (TELEFONO_RECEPTOR.ToString().Trim() == "" || TELEFONO_RECEPTOR.ToString().Trim() == "0")
                        {
                            rec.telefono = null;
                        }
                        else
                        {
                            rec.telefono = TELEFONO_RECEPTOR;
                        }

                        if (EMAIL_RECEPTOR.ToString().Trim() == "")
                         {
                            rec.correo = "eurosaldte@gmail.com";
                        }
                         else
                         {
                             rec.correo = EMAIL_RECEPTOR;
                         }
                        
                        rec.direccion = direccion_receptor;

                    }
                }


                if (TIPO_DOCUMENTO == "6")
                {

                    if (HOMOLOGADO_DUI_NIT.ToString().Trim() == "S")
                    {
                        rec.tipoDocumento = "36";
                        rec.numDocumento = NUMERO_CEDULA_RECEPTOR.Replace("-", "").Trim();
                    }
                    else
                    {
                        if (NUMERO_CEDULA_RECEPTOR.ToString().Trim() == "" && NIT_RECEPTOR.ToString().Trim() == "")
                        {
                            rec.tipoDocumento = null;
                            rec.numDocumento = null;

                        }
                        else
                        {

                            if (NUMERO_CEDULA_RECEPTOR.ToString().Trim() == "")
                            {
                                rec.tipoDocumento = "36";
                                rec.numDocumento = NIT_RECEPTOR;

                            }
                            else
                            {
                                rec.tipoDocumento = "13";
                                rec.numDocumento = NUMERO_CEDULA_RECEPTOR;
                            }




                        }



                    }







                    if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nrc = null;
                    }
                    else
                    {
                        rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                    }






                   // direccion_receptor.Add(adddireccionreceptor(DEPARTAMENTO_RECEPTOR, MUNICIPIO_RECEPTOR, DIRECCION_RECEPTOR));

                    if (NUMERO_REGISTRO_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.nrc = null;
                    }
                    else
                    {
                        rec.nrc = NUMERO_REGISTRO_RECEPTOR;
                    }
                    if (CODIGO_ACTECON.ToString().Trim() == "")
                    {
                        rec.codActividad = "10005";
                        rec.descActividad = "OTROS";

                    }
                    else
                    {
                        rec.codActividad = CODIGO_ACTIVIDAD;
                        rec.descActividad = CODIGO_DESCRIPCION;
                    }



                    rec.nombre = RAZON_SOCIAL_RECEPTOR;
                    rec.nombreComercial = NOMBRE_COMERCIAL_RECEPTOR;
                    rec.tipoPersona = 1;
                    rec.complemento = DIRECCION_CLIENTE.Trim();
                    rec.nombrePais = "El Salvador";
                    rec.codPais = "9300";


                    if (TELEFONO_RECEPTOR.ToString().Trim() == "" || TELEFONO_RECEPTOR.ToString().Trim() == "0")
                    {
                        rec.telefono = null;
                    }
                    else
                    {
                        rec.telefono = TELEFONO_RECEPTOR;
                    }

                    
                    if (EMAIL_RECEPTOR.ToString().Trim() == "")
                    {
                        rec.correo = "eurosaldte@gmail.com";
                    }
                    else
                    {
                        rec.correo = EMAIL_RECEPTOR;
                    }
                  
                  

                    rec.direccion = null;
                }

                receptor.Add(rec);


                //APARTADO CUERPO DE DOCUMENTO

                detalle detalle = new detalle();
                List<CuerpoDocumento> cuerpo_documents = new List<CuerpoDocumento>();

                if (TIPO_DOCUMENTO == "1")
                {
                   
                    foreach (var i in detalle_factura)
                    {
                        int correlativo = Convert.ToInt32(i[0].ToString());
                        string articulo = i[1].ToString();
                        double cantidad = Convert.ToDouble(i[2].ToString());
                        string descripcion = Convert.ToString(i[3].ToString());
                        double precio_unitario = Convert.ToDouble(i[4]);
                        double venta_gravada = Convert.ToDouble(i[5]);
                        double monto_descuento = Convert.ToDouble(i[6]);
                        double ivaItem= Math.Round(Convert.ToDouble(i[5])-(Convert.ToDouble(i[5])/1.13),2);
                        int uniMedida = 59;

                        if (articulo.ToString().Trim() == "COM")
                        {
                            uniMedida = 99;
                            cantidad = 1;
                        }

                        if (cantidad == 0)
                        {
                            uniMedida = 99;
                            cantidad = 1;
                            precio_unitario = 0;

                        }

                        cuerpo_documents.Add(adddetalle(correlativo, 1, null, articulo, null, descripcion, cantidad, uniMedida, precio_unitario, monto_descuento, 0, 0, venta_gravada, null, 0, 0, ivaItem));



                    }
                }


                if (TIPO_DOCUMENTO == "2")
                {
                    List<string> lista = Regex.Split("20", @"\s+").ToList();

                    foreach (var i in detalle_factura)
                    {
                        int correlativo = Convert.ToInt32(i[0].ToString());
                        string articulo = i[1].ToString();
                        double cantidad = Convert.ToDouble(i[2].ToString());
                        string descripcion = Convert.ToString(i[3].ToString());
                        double precio_unitario = Convert.ToDouble(i[4]);
                        double venta_gravada = Convert.ToDouble(i[5]);
                        double monto_descuento = Convert.ToDouble(i[6]);
                        int uniMedida = 59;

                        if (articulo.ToString().Trim() == "COM")
                        {
                            uniMedida = 99;
                            cantidad = 1;
                        }


                        if (cantidad == 0)
                        {
                            uniMedida = 99;
                            cantidad = 1;
                            precio_unitario = 0;

                        }

                        cuerpo_documents.Add(adddetalle(correlativo, 1, null, articulo, null, descripcion, cantidad, uniMedida, precio_unitario, monto_descuento, 0, 0, venta_gravada, lista, 0, 0, 0));
                        
                    }
                }
                if (TIPO_DOCUMENTO == "3" && Convert.ToInt32(FACTURA_ID_ABONONC)>0)
                {
                    List<string> lista = Regex.Split("20", @"\s+").ToList();

                    foreach (var i in detalle_factura)
                    {
                        int correlativo = Convert.ToInt32(i[0].ToString());
                        string articulo = i[1].ToString();
                        double cantidad = Convert.ToDouble(i[2].ToString());
                        string descripcion = Convert.ToString(i[3].ToString());
                        double precio_unitario = Convert.ToDouble(i[4]);
                        double venta_gravada = Convert.ToDouble(i[5]);
                        double monto_descuento = Convert.ToDouble(i[6]);
                        int uniMedida = 59;

                        if (articulo.ToString().Trim() == "COM")
                        {
                            uniMedida = 99;
                            cantidad = 1;
                        }

                        if (cantidad == 0)
                        {
                            uniMedida = 99;
                            cantidad = 1;
                            precio_unitario = 0;

                        }

                        cuerpo_documents.Add(adddetalle(correlativo, 1, NDOCUMENTO_RELACIONADO, articulo, null, descripcion, cantidad, uniMedida, precio_unitario, monto_descuento, 0, 0, venta_gravada, lista, 0, 0, 0));
                        
                    }
                }

                if (TIPO_DOCUMENTO == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    List<string> lista = Regex.Split("20", @"\s+").ToList();

                    foreach (var i in detalle_factura)
                    {
                        int correlativo = Convert.ToInt32(i[0].ToString());
                        string articulo = i[1].ToString();
                        double cantidad = Convert.ToDouble(i[2].ToString());
                        string descripcion = Convert.ToString(i[3].ToString());
                        double precio_unitario = Convert.ToDouble(i[4]);
                        double venta_gravada = Convert.ToDouble(i[5]);
                        double monto_descuento = Convert.ToDouble(i[6]);
                        int uniMedida = 59;

                        if (articulo.ToString().Trim() == "COM")
                        {
                            uniMedida = 99;
                            cantidad = 1;
                        }

                        if (cantidad == 0)
                        {
                            uniMedida = 99;
                            cantidad = 1;
                            precio_unitario = 0;

                        }

                        cuerpo_documents.Add(adddetalle(correlativo, 1, NDOCUMENTO_RELACIONADO, articulo, null, descripcion, cantidad, uniMedida, precio_unitario, monto_descuento, 0, 0, venta_gravada, lista, 0, 0, 0));

                    }
                }

                if (TIPO_DOCUMENTO == "6")
                {
                    List<string> lista = Regex.Split("C3", @"\s+").ToList();

                    foreach (var i in detalle_factura)
                    {
                        int correlativo = Convert.ToInt32(i[0].ToString());
                        string articulo = i[1].ToString();
                        double cantidad = Convert.ToDouble(i[2].ToString());
                        string descripcion = Convert.ToString(i[3].ToString());
                        double precio_unitario = Convert.ToDouble(i[4]);
                        double venta_gravada = Convert.ToDouble(i[5]);
                        double monto_descuento = Convert.ToDouble(i[6]);
                        double ivaItem = Math.Round(Convert.ToDouble(i[5]) - (Convert.ToDouble(i[5]) / 1.13), 2);
                        int uniMedida = 59;

                        if (articulo.ToString().Trim() == "COM")
                        {
                            uniMedida = 99;
                            cantidad = 1;
                        }

                        if (cantidad == 0)
                        {
                            uniMedida = 99;
                            cantidad = 1;
                            precio_unitario = 0;

                        }

                        cuerpo_documents.Add(adddetalle(correlativo, 1, null, articulo, null, descripcion, cantidad, uniMedida, precio_unitario, monto_descuento, 0, 0, venta_gravada, lista, 0, 0, ivaItem));



                    }
                }



                detalle.cuerpoDocumento = cuerpo_documents;
                    detalles.Add(detalle);
                

                //APARTADO RESUMEN

                Resumen res = new Resumen();
                List<Tributo> tributos = new List<Tributo>();

                if (TIPO_DOCUMENTO == "1")
                {
                    res.totalGravada = Convert.ToDouble(STOTALG_SUMASFACT);
                    res.totalExenta = Convert.ToDouble(VALOR_EXENTO);
                    res.montoTotalOperacion = Convert.ToDouble(STOTALG_SUMASFACT);
                    res.totalNoSuj = Convert.ToDouble(VALOR_NOSUJETO);
                    res.ivaRete1 = Convert.ToDouble(VALOR_RETE);
                    res.subTotalVentas = Convert.ToDouble(STOTALG_SUMASFACT);
                    // res.descuGravada = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.totalDescu = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.subTotal = Convert.ToDouble(STOTALG_SUMASFACT);
                    res.totalPagar = Convert.ToDouble(TOTAL_FACTURA);
                    res.tributos = null;
                    res.condicionOperacion = Convert.ToInt32(CREDITO_CONTADO);
                    decimal totalw = Decimal.Parse(TOTAL_FACTURA);
                    res.totalLetras = Conversores.NumeroALetras(totalw);
                    res.totalIva = Math.Round(Convert.ToDouble(STOTALG_SUMASFACT) - (Convert.ToDouble(STOTALG_SUMASFACT) / 1.13), 2);

                }
               
                if (TIPO_DOCUMENTO == "2")
                {
                    res.totalGravada = Convert.ToDouble(VALOR_GRABADO);
                    res.totalExenta = Convert.ToDouble(VALOR_EXENTO);

                    if (SUMA_GRAVA_MASIVA.ToString().Trim() == "")
                    {
                        res.montoTotalOperacion = Convert.ToDouble(VALOR_GRABADO);
                    }
                    else
                    {
                        res.montoTotalOperacion = Convert.ToDouble(SUMA_GRAVA_MASIVA);
                    }
                   


                    res.totalNoSuj = Convert.ToDouble(VALOR_NOSUJETO);
                    res.ivaRete1 = Convert.ToDouble(VALOR_RETE);
                    res.subTotalVentas = Convert.ToDouble(VALOR_GRABADO);
                    // res.descuGravada = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.totalDescu = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.subTotal = Convert.ToDouble(VALOR_GRABADO);
                    res.totalPagar = Convert.ToDouble(TOTAL_FACTURA);
                    tributos.Add(addtributos("20" +
                        "", "Impuesto al Valor Agregado 13%", Convert.ToDouble(VALOR_IVA)));
                    res.tributos = tributos;
                    res.condicionOperacion = Convert.ToInt32(CREDITO_CONTADO);
                    decimal totalw = Decimal.Parse(TOTAL_FACTURA);
                    res.totalLetras = Conversores.NumeroALetras(totalw);
                    
                }
                if (TIPO_DOCUMENTO == "3" && Convert.ToInt32(FACTURA_ID_ABONONC)>0)
                {
                    res.totalGravada = Convert.ToDouble(VALOR_GRABADO);
                    res.totalExenta = Convert.ToDouble(VALOR_EXENTO);
                    res.montoTotalOperacion = Convert.ToDouble(TOTAL_FACTURA);

                    res.totalNoSuj = Convert.ToDouble(VALOR_NOSUJETO);
                    res.ivaRete1 = Convert.ToDouble(VALOR_RETE);
                    res.subTotalVentas = Convert.ToDouble(VALOR_GRABADO);
                    // res.descuGravada = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.totalDescu = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.subTotal = Convert.ToDouble(VALOR_GRABADO);
                    res.totalPagar = Convert.ToDouble(TOTAL_FACTURA);
                    tributos.Add(addtributos("20" +
                        "", "Impuesto al Valor Agregado 13%", Convert.ToDouble(VALOR_IVA)));
                    res.tributos = tributos;
                    res.condicionOperacion = Convert.ToInt32(CREDITO_CONTADO);
                    decimal totalw = Decimal.Parse(TOTAL_FACTURA);
                    res.totalLetras = Conversores.NumeroALetras(totalw);

                }
                if (TIPO_DOCUMENTO == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    res.totalGravada = Convert.ToDouble(VALOR_GRABADO);
                    res.totalExenta = Convert.ToDouble(VALOR_EXENTO);
                    res.montoTotalOperacion = Convert.ToDouble(TOTAL_FACTURA);

                    res.totalNoSuj = Convert.ToDouble(VALOR_NOSUJETO);
                    res.ivaRete1 = Convert.ToDouble(VALOR_RETE);
                    res.subTotalVentas = Convert.ToDouble(VALOR_GRABADO);
                    // res.descuGravada = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.totalDescu = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.subTotal = Convert.ToDouble(VALOR_GRABADO);
                    res.totalPagar = Convert.ToDouble(TOTAL_FACTURA);
                    tributos.Add(addtributos("20" +
                        "", "Impuesto al Valor Agregado 13%", Convert.ToDouble(VALOR_IVA)));
                    res.tributos = tributos;
                    res.condicionOperacion = Convert.ToInt32(CREDITO_CONTADO);
                    decimal totalw = Decimal.Parse(TOTAL_FACTURA);
                    res.totalLetras = Conversores.NumeroALetras(totalw);

                }

                if (TIPO_DOCUMENTO == "6")
                {
                    res.totalGravada = Convert.ToDouble(TOTAL_FACTURA);
                    res.totalExenta = Convert.ToDouble(VALOR_EXENTO);
                    res.montoTotalOperacion = Convert.ToDouble(TOTAL_FACTURA);
                    res.totalNoSuj = Convert.ToDouble(VALOR_NOSUJETO);
                    res.ivaRete1 = Convert.ToDouble(VALOR_RETE);
                    res.subTotalVentas = Convert.ToDouble(TOTAL_FACTURA);
                    // res.descuGravada = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.totalDescu = Convert.ToDouble(VALOR_DESCUENTOS);
                    res.subTotal = Convert.ToDouble(TOTAL_FACTURA);
                    res.totalPagar = Convert.ToDouble(TOTAL_FACTURA);
                    res.tributos = null;
                    res.condicionOperacion = Convert.ToInt32(CREDITO_CONTADO);
                    decimal totalw = Decimal.Parse(TOTAL_FACTURA);
                    res.totalLetras = Conversores.NumeroALetras(totalw);
                    res.totalIva = Math.Round(Convert.ToDouble(TOTAL_FACTURA) - (Convert.ToDouble(TOTAL_FACTURA) / 1.13), 2);


                }


                resumen.Add(res);

                //APARTADO DETALLE EXTENSION

                detalle_extension de = new detalle_extension();
                List<Extension> exts = new List<Extension>();

                // exts.Add(addExtension("", "", "", "", "", ""));

                if (exts.Count <= 0)
                {
                    de.extension = null;
                }
                else
                {
                    de.extension = exts;
                }

                detalle_extension.Add(de);

                //APARTADO DETALLE_APENDICE

                detalle_apendice da = new detalle_apendice();
                List<Apendice> apend = new List<Apendice>();

              

                if (REGISTRO_FISCAL.ToString().Trim() != "")
                {
                    apend.Add(addApendice("CÓDIGO CLIENTE", "CÓDIGO CLIENTE", REGISTRO_FISCAL.ToString().Trim()));
                }
                if (CREDITO_.ToString().Trim() == "2")
                {
                    apend.Add(addApendice("CONDICIÓN DE PAGO", "CONDICIÓN DE PAGO", "CREDITO "+ DIAS_CREDITO.ToString().Trim()+" DIAS"));
                }
                if (VENDEDOR.ToString().Trim() !="")
                {
                    string querytablaEMPLEADO = "Select NOMBRE From VENDEDORES where VENDEDOR=" + VENDEDOR + "";

                    cxdb1.Open();
                    SqlCommand cmdempleado = new SqlCommand(querytablaEMPLEADO, cxdb1);
                    string NOMBREVENDEDOR;
                    using (SqlDataReader reader = cmdempleado.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            NOMBREVENDEDOR = Convert.ToString(reader["NOMBRE"]);
                            apend.Add(addApendice("VENDEDOR", "VENDEDOR", VENDEDOR.ToString().Trim() + "-" + NOMBREVENDEDOR.ToString().Trim()));
                        }
                    }
                   
                }
                if (FACTURA_ID.ToString().Trim() != "")
                {
                    apend.Add(addApendice("FACTURA ID", "FACTURA ID",FACTURA_ID ));
                }

                if (CODIGO_SUCURSAL_CLIENTE.ToString().Trim() != "")
                {
                    apend.Add(addApendice("SUCURSAL CLIENTE", "SUCURSAL CLIENTE", CODIGO_SUCURSAL_CLIENTE.ToString().Trim()));
                }
                

                if (TIPO_DOCUMENTO == "1")
                {
                    apend.Add(addApendice("1MPR1M3", "1 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "2 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "3 copias", "S"));


                    if (NOMBRE_COMERCIAL_RECEPTOR.ToString().Trim() != "")
                    {
                        apend.Add(addApendice("NOMBRECOMERCIAL", "NOMBRECOMERCIAL", NOMBRE_COMERCIAL_RECEPTOR));

                    }

                }
                if (TIPO_DOCUMENTO == "2")
                {
                    apend.Add(addApendice("1MPR1M3", "1 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "2 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "3 copias", "S"));


                }
                else if (TIPO_DOCUMENTO == "3" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    apend.Add(addApendice("1MPR1M3", "1 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "2 copias", "S"));
                }

                if (TIPO_DOCUMENTO == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {
                    apend.Add(addApendice("1MPR1M3", "1 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "2 copias", "S"));

                }
                if (TIPO_DOCUMENTO == "6")
                {
                    apend.Add(addApendice("1MPR1M3", "1 copias", "S"));
                    apend.Add(addApendice("1MPR1M3", "2 copias", "S"));

                }




                cxdb1.Close();



                if (apend.Count <= 0)
                {
                    da.apendice = null;
                }
                else
                {
                    da.apendice = apend;
                }

                detalle_apendice.Add(da);




                string str_identificacion = JsonConvert.SerializeObject(iden, Formatting.Indented);
                string str_emisor = JsonConvert.SerializeObject(emi, Formatting.Indented);
                string str_receptor = JsonConvert.SerializeObject(rec, Formatting.Indented);
                string str_cuerpodoc = JsonConvert.SerializeObject(detalles, Formatting.Indented);
                string cadenaRecortada1 = str_cuerpodoc.Remove(str_cuerpodoc.Length - 5);
                string cadenaRecortada2 = cadenaRecortada1.Remove(0, 6);
                string str_resumen = JsonConvert.SerializeObject(resumen, Formatting.Indented);
                string cadenaRecortada1_resumen = str_resumen.Remove(str_resumen.Length - 1);
                string cadenaRecortada2_resumen = cadenaRecortada1_resumen.Remove(0, 1);

                string str_relacionado = JsonConvert.SerializeObject(dt_re, Formatting.Indented);
                string cadenaRecortada1_relacionado = str_relacionado.Remove(str_relacionado.Length - 5);
                string cadenaRecortada2_relacionado = cadenaRecortada1_relacionado.Remove(0, 6);


                string str_extension = JsonConvert.SerializeObject(detalle_extension, Formatting.Indented);
                string cadenaRecortada1_extension = str_extension.Remove(str_extension.Length - 5);
                string cadenaRecortada2_extension = cadenaRecortada1_extension.Remove(0, 6);


                string str_apendice = JsonConvert.SerializeObject(detalle_apendice, Formatting.Indented);
                string cadenaRecortada1_apendice = str_apendice.Remove(str_apendice.Length - 5);
                string cadenaRecortada2_apendice = cadenaRecortada1_apendice.Remove(0, 6);







                string var_identificacion = "\"identificacion\":" + str_identificacion;
                string var_emisor = "\"emisor\":" + str_emisor;
                string var_receptor = "\"receptor\":" + str_receptor;
                string var_otrosDocumentos = "\"otrosDocumentos\":" + "null";
                string var_ventaTercero = "\"ventaTercero\":" + "null";
                string var_cuerpo = cadenaRecortada2.Trim();
                string var_resumen = "\"resumen\":" + cadenaRecortada2_resumen.Trim();
                string var_extension = cadenaRecortada2_extension.Trim();
                string var_apendice = cadenaRecortada2_apendice.Trim();

                string json_completo = "{\n" + var_identificacion + ",\n" + cadenaRecortada2_relacionado.Trim() + ",\n" + var_emisor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_receptor.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_otrosDocumentos + ",\n" + var_ventaTercero + ",\n" + var_cuerpo + ",\n" + var_resumen + ",\n" + var_extension.Replace("[", "").Trim().Replace("]", "").Trim() + ",\n" + var_apendice + "\n}";






                //ACTUALIZAR  TABLA FACTURA//


                string query = "UPDATE FACTURAS SET ESTA_IMPRESA = @ESTA_IMPRESA WHERE FACTURA_ID='" + FACTURA_ID + "'";


                cxdb1.Open();

                using (SqlCommand command = new SqlCommand(query, cxdb1))
                {

                    command.Parameters.AddWithValue("@ESTA_IMPRESA", 1);

                    int rowsAffected = command.ExecuteNonQuery();


                }
                cxdb1.Close();

                //INSERTAR EN TABLA RESPUESTA EUROSAL


                if (TIPO_DOCUMENTO == "1")
                {



                    string queryEuro = "If Not Exists(select * from RESPUESTA_EUROSAL where factura_id='" + FACTURA_ID +"') Begin INSERT INTO RESPUESTA_EUROSAL(version,ambiente,versionApp,estado,factura_id,codigoGeneracion,numeroControl,tipoDte,selloRecibido,fhProcesamiento,clasificaMsg,codigoMsg,descripcionMsg,firma,procesado) VALUES (@version,@ambiente,@versionApp,@estado,@factura_id,@codigoGeneracion,@numeroControl,@tipoDte,@selloRecibido,@fhProcesamiento,@clasificaMsg,@codigoMsg,@descripcionMsg,@firma,@procesado) End;";
                    cxdb2.Open();
                    using (SqlCommand command = new SqlCommand(queryEuro, cxdb2))
                    {

                        command.Parameters.AddWithValue("@version", 1);
                        command.Parameters.AddWithValue("@ambiente", ambiente);
                        command.Parameters.AddWithValue("@versionApp", "2");
                        command.Parameters.AddWithValue("@estado", "");
                        command.Parameters.AddWithValue("@factura_id", FACTURA_ID);
                        command.Parameters.AddWithValue("@codigoGeneracion", codigoGeneracion);
                        command.Parameters.AddWithValue("@numeroControl", numeroControl);
                        command.Parameters.AddWithValue("@tipoDte", TIPODTE);
                        command.Parameters.AddWithValue("@selloRecibido", "");
                        command.Parameters.AddWithValue("@fhProcesamiento", "");
                        command.Parameters.AddWithValue("@clasificaMsg", "");
                        command.Parameters.AddWithValue("@codigoMsg", "");
                        command.Parameters.AddWithValue("@descripcionMsg", "");
                        command.Parameters.AddWithValue("@firma", "");
                        command.Parameters.AddWithValue("@procesado", 0);


                        int rowsAffected = command.ExecuteNonQuery();


                    }
                    cxdb2.Close();



                    System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", json_completo.Trim());
                }
                if (TIPO_DOCUMENTO == "2")
                {

                    string queryEuro = "If Not Exists(select * from RESPUESTA_EUROSAL where factura_id='" + FACTURA_ID + "') Begin INSERT INTO RESPUESTA_EUROSAL(version,ambiente,versionApp,estado,factura_id,codigoGeneracion,numeroControl,tipoDte,selloRecibido,fhProcesamiento,clasificaMsg,codigoMsg,descripcionMsg,firma,procesado) VALUES (@version,@ambiente,@versionApp,@estado,@factura_id,@codigoGeneracion,@numeroControl,@tipoDte,@selloRecibido,@fhProcesamiento,@clasificaMsg,@codigoMsg,@descripcionMsg,@firma,@procesado) End;";
                    cxdb2.Open();
                    using (SqlCommand command = new SqlCommand(queryEuro, cxdb2))
                    {

                        command.Parameters.AddWithValue("@version", 1);
                        command.Parameters.AddWithValue("@ambiente", ambiente);
                        command.Parameters.AddWithValue("@versionApp", "2");
                        command.Parameters.AddWithValue("@estado", "");
                        command.Parameters.AddWithValue("@factura_id", FACTURA_ID);
                        command.Parameters.AddWithValue("@codigoGeneracion", codigoGeneracion);
                        command.Parameters.AddWithValue("@numeroControl", numeroControl);
                        command.Parameters.AddWithValue("@tipoDte", TIPODTE);
                        command.Parameters.AddWithValue("@selloRecibido", "");
                        command.Parameters.AddWithValue("@fhProcesamiento", "");
                        command.Parameters.AddWithValue("@clasificaMsg", "");
                        command.Parameters.AddWithValue("@codigoMsg", "");
                        command.Parameters.AddWithValue("@descripcionMsg", "");
                        command.Parameters.AddWithValue("@firma", "");
                        command.Parameters.AddWithValue("@procesado", 0);


                        int rowsAffected = command.ExecuteNonQuery();


                    }
                    cxdb2.Close();



                    System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", json_completo.Trim());
                }
                if (TIPO_DOCUMENTO == "3" && Convert.ToInt32(FACTURA_ID_ABONONC)>0)
                {

                    string queryEuro = "If Not Exists(select * from RESPUESTA_EUROSAL where factura_id='" + FACTURA_ID + "') Begin INSERT INTO RESPUESTA_EUROSAL(version,ambiente,versionApp,estado,factura_id,codigoGeneracion,numeroControl,tipoDte,selloRecibido,fhProcesamiento,clasificaMsg,codigoMsg,descripcionMsg,firma,procesado) VALUES (@version,@ambiente,@versionApp,@estado,@factura_id,@codigoGeneracion,@numeroControl,@tipoDte,@selloRecibido,@fhProcesamiento,@clasificaMsg,@codigoMsg,@descripcionMsg,@firma,@procesado) End ;";
                    cxdb2.Open();
                    using (SqlCommand command = new SqlCommand(queryEuro, cxdb2))
                    {

                        command.Parameters.AddWithValue("@version", 1);
                        command.Parameters.AddWithValue("@ambiente", ambiente);
                        command.Parameters.AddWithValue("@versionApp", "2");
                        command.Parameters.AddWithValue("@estado", "");
                        command.Parameters.AddWithValue("@factura_id", FACTURA_ID);
                        command.Parameters.AddWithValue("@codigoGeneracion", codigoGeneracion);
                        command.Parameters.AddWithValue("@numeroControl", numeroControl);
                        command.Parameters.AddWithValue("@tipoDte", TIPODTE);
                        command.Parameters.AddWithValue("@selloRecibido", "");
                        command.Parameters.AddWithValue("@fhProcesamiento", "");
                        command.Parameters.AddWithValue("@clasificaMsg", "");
                        command.Parameters.AddWithValue("@codigoMsg", "");
                        command.Parameters.AddWithValue("@descripcionMsg", "");
                        command.Parameters.AddWithValue("@firma", "");
                        command.Parameters.AddWithValue("@procesado", 0);


                        int rowsAffected = command.ExecuteNonQuery();


                    }
                    cxdb2.Close();



                    System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", json_completo.Trim());
                }

                if (TIPO_DOCUMENTO == "4" && Convert.ToInt32(FACTURA_ID_ABONONC) > 0)
                {

                    string queryEuro = "If Not Exists(select * from RESPUESTA_EUROSAL where factura_id='" + FACTURA_ID + "') Begin INSERT INTO RESPUESTA_EUROSAL(version,ambiente,versionApp,estado,factura_id,codigoGeneracion,numeroControl,tipoDte,selloRecibido,fhProcesamiento,clasificaMsg,codigoMsg,descripcionMsg,firma,procesado) VALUES (@version,@ambiente,@versionApp,@estado,@factura_id,@codigoGeneracion,@numeroControl,@tipoDte,@selloRecibido,@fhProcesamiento,@clasificaMsg,@codigoMsg,@descripcionMsg,@firma,@procesado) End ;";
                    cxdb2.Open();
                    using (SqlCommand command = new SqlCommand(queryEuro, cxdb2))
                    {

                        command.Parameters.AddWithValue("@version", 1);
                        command.Parameters.AddWithValue("@ambiente", ambiente);
                        command.Parameters.AddWithValue("@versionApp", "2");
                        command.Parameters.AddWithValue("@estado", "");
                        command.Parameters.AddWithValue("@factura_id", FACTURA_ID);
                        command.Parameters.AddWithValue("@codigoGeneracion", codigoGeneracion);
                        command.Parameters.AddWithValue("@numeroControl", numeroControl);
                        command.Parameters.AddWithValue("@tipoDte", TIPODTE);
                        command.Parameters.AddWithValue("@selloRecibido", "");
                        command.Parameters.AddWithValue("@fhProcesamiento", "");
                        command.Parameters.AddWithValue("@clasificaMsg", "");
                        command.Parameters.AddWithValue("@codigoMsg", "");
                        command.Parameters.AddWithValue("@descripcionMsg", "");
                        command.Parameters.AddWithValue("@firma", "");
                        command.Parameters.AddWithValue("@procesado", 0);


                        int rowsAffected = command.ExecuteNonQuery();


                    }
                    cxdb2.Close();



                    System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", json_completo.Trim());
                }


                if (TIPO_DOCUMENTO == "6")
                {

                    string queryEuro = "If Not Exists(select * from RESPUESTA_EUROSAL where factura_id='" + FACTURA_ID + "') Begin INSERT INTO RESPUESTA_EUROSAL(version,ambiente,versionApp,estado,factura_id,codigoGeneracion,numeroControl,tipoDte,selloRecibido,fhProcesamiento,clasificaMsg,codigoMsg,descripcionMsg,firma,procesado) VALUES (@version,@ambiente,@versionApp,@estado,@factura_id,@codigoGeneracion,@numeroControl,@tipoDte,@selloRecibido,@fhProcesamiento,@clasificaMsg,@codigoMsg,@descripcionMsg,@firma,@procesado) End ;";
                    cxdb2.Open();
                    using (SqlCommand command = new SqlCommand(queryEuro, cxdb2))
                    {

                        command.Parameters.AddWithValue("@version", 1);
                        command.Parameters.AddWithValue("@ambiente",ambiente);
                        command.Parameters.AddWithValue("@versionApp", "2");
                        command.Parameters.AddWithValue("@estado", "");
                        command.Parameters.AddWithValue("@factura_id", FACTURA_ID);
                        command.Parameters.AddWithValue("@codigoGeneracion", codigoGeneracion);
                        command.Parameters.AddWithValue("@numeroControl", numeroControl);
                        command.Parameters.AddWithValue("@tipoDte", TIPODTE);
                        command.Parameters.AddWithValue("@selloRecibido", "");
                        command.Parameters.AddWithValue("@fhProcesamiento", "");
                        command.Parameters.AddWithValue("@clasificaMsg", "");
                        command.Parameters.AddWithValue("@codigoMsg", "");
                        command.Parameters.AddWithValue("@descripcionMsg", "");
                        command.Parameters.AddWithValue("@firma", "");
                        command.Parameters.AddWithValue("@procesado", 0);


                        int rowsAffected = command.ExecuteNonQuery();


                    }
                    cxdb2.Close();



                    System.IO.File.WriteAllText("C:\\archivo\\" + codigoGeneracion + ".json", json_completo.Trim());
                }




                NUME_DOCUMENTO = ""; FECHA_EMISION = ""; HORA_EMISION = ""; REGISTRO_FISCAL = ""; TIPO_DOCUMENTO = ""; FACTURA_ID = ""; FACTURA_ID_ABONONC = ""; NOTAS = "";
                casaMatriz = ""; numeroControl_correlativo = ""; CODIGO_GENERACIONDTE =""; NUMERO_CONTROLDTE = "";
                codigoGeneracion = ""; TIPODTE = ""; numeroControl = "";
                HOMOLOGADO_DUI_NIT = ""; CODIGO_TIPOIDREC = ""; NUMERO_CEDULA_RECEPTOR = ""; NIT_RECEPTOR = ""; NUMERO_REGISTRO_RECEPTOR = ""; RAZON_SOCIAL_RECEPTOR = ""; NOMBRE_COMERCIAL_RECEPTOR = "";
                CODIGO_ACTECON = ""; EMAIL_RECEPTOR = ""; TELEFONO_RECEPTOR = ""; MUNICIPIO_RECEPTOR = ""; DEPARTAMENTO_RECEPTOR = ""; DIRECCION_RECEPTOR = "";
                CODIGO_ACTIVIDAD = ""; CODIGO_DESCRIPCION = "";
                CREDITO_CONTADO = ""; TOTAL_FACTURA = ""; VALOR_EXENTO = ""; VALOR_GRABADO = ""; SUMA_GRAVA_MASIVA = ""; VALOR_NOSUJETO = ""; VALOR_IVA = ""; VALOR_RETE = ""; VALOR_DESCUENTOS = ""; CODCLIENTE = "";
                VENDEDOR = ""; CREDITO_ = ""; DIAS_CREDITO = "";
                FECHA_REAL_CREACION_RELACIONADO = ""; TIPO_DOCUMENTO_RELACIONADO = ""; NDOCUMENTO_RELACIONADO = ""; STOTALG_SUMASFACT = "";
                DIRECCION_CLIENTE = "";

                identificacion.Clear();
                dt_re.Clear();
                emisor.Clear();
                receptor.Clear();
                detalles.Clear();
                detalle_extension.Clear();
                resumen.Clear();
                detalle_factura.Clear();
                detalle_apendice.Clear();


              
                
            }
            else
            {
                //------------------------NO TIENE DATOS----------------------------//

                NUME_DOCUMENTO = ""; FECHA_EMISION = ""; HORA_EMISION = ""; REGISTRO_FISCAL = ""; TIPO_DOCUMENTO = ""; FACTURA_ID = ""; FACTURA_ID_ABONONC = ""; NOTAS = "";
                casaMatriz = ""; numeroControl_correlativo = ""; CODIGO_GENERACIONDTE = ""; NUMERO_CONTROLDTE = "";

                codigoGeneracion = "";
                HOMOLOGADO_DUI_NIT = ""; CODIGO_TIPOIDREC = ""; NUMERO_CEDULA_RECEPTOR = ""; NIT_RECEPTOR = ""; NUMERO_REGISTRO_RECEPTOR = ""; RAZON_SOCIAL_RECEPTOR = ""; NOMBRE_COMERCIAL_RECEPTOR = "";
                CODIGO_ACTECON = ""; EMAIL_RECEPTOR = ""; TELEFONO_RECEPTOR = ""; MUNICIPIO_RECEPTOR = ""; DEPARTAMENTO_RECEPTOR = ""; DIRECCION_RECEPTOR = "";
                CODIGO_ACTIVIDAD = ""; CODIGO_DESCRIPCION = "";
                CREDITO_CONTADO = ""; TOTAL_FACTURA = ""; VALOR_EXENTO = ""; VALOR_GRABADO = ""; SUMA_GRAVA_MASIVA = ""; VALOR_NOSUJETO = ""; VALOR_IVA = ""; VALOR_RETE = ""; VALOR_DESCUENTOS = ""; CODCLIENTE = "";

                VENDEDOR = ""; CREDITO_ = ""; DIAS_CREDITO = "";
                FECHA_REAL_CREACION_RELACIONADO = ""; TIPO_DOCUMENTO_RELACIONADO = ""; NDOCUMENTO_RELACIONADO = ""; STOTALG_SUMASFACT = "";
                DIRECCION_CLIENTE = "";

                identificacion.Clear();
                dt_re.Clear();
                emisor.Clear();
                receptor.Clear();
                detalles.Clear();
                detalle_extension.Clear();
                resumen.Clear();
                detalle_factura.Clear();
                detalle_apendice.Clear();

            }

            //-------------------------FIN CREACION JSON----------------------------//

            cxdb1.Close();

            NUME_DOCUMENTO = ""; FECHA_EMISION = ""; HORA_EMISION = ""; REGISTRO_FISCAL = ""; TIPO_DOCUMENTO = ""; FACTURA_ID = ""; FACTURA_ID_ABONONC = ""; NOTAS = "";
            casaMatriz = ""; numeroControl_correlativo = "";

            codigoGeneracion = ""; CODIGO_GENERACIONDTE = ""; NUMERO_CONTROLDTE = "";
            HOMOLOGADO_DUI_NIT = ""; CODIGO_TIPOIDREC = ""; NUMERO_CEDULA_RECEPTOR = ""; NIT_RECEPTOR = ""; NUMERO_REGISTRO_RECEPTOR = ""; RAZON_SOCIAL_RECEPTOR = ""; NOMBRE_COMERCIAL_RECEPTOR = "";
            CODIGO_ACTECON = ""; EMAIL_RECEPTOR = ""; TELEFONO_RECEPTOR = ""; MUNICIPIO_RECEPTOR = ""; DEPARTAMENTO_RECEPTOR = ""; DIRECCION_RECEPTOR = "";
            CODIGO_ACTIVIDAD = ""; CODIGO_DESCRIPCION = "";
            CREDITO_CONTADO = ""; TOTAL_FACTURA = ""; VALOR_EXENTO = ""; VALOR_GRABADO = ""; SUMA_GRAVA_MASIVA = ""; VALOR_NOSUJETO = ""; VALOR_IVA = ""; VALOR_RETE = ""; VALOR_DESCUENTOS = ""; CODCLIENTE = "";
            VENDEDOR = ""; CREDITO_ = ""; DIAS_CREDITO = "";
            FECHA_REAL_CREACION_RELACIONADO = ""; TIPO_DOCUMENTO_RELACIONADO = ""; NDOCUMENTO_RELACIONADO = ""; STOTALG_SUMASFACT = "";
            DIRECCION_CLIENTE = "";

            identificacion.Clear();
            dt_re.Clear();
            emisor.Clear();
            receptor.Clear();
            detalles.Clear();
            detalle_extension.Clear();
            resumen.Clear();
            detalle_factura.Clear();
            detalle_apendice.Clear();




        }


        private direccion adddireccionemisor(string departamento, string municipio, string complemento)
        {
            direccion direccion = new direccion();
            direccion.departamento=departamento.ToString().Trim();
            direccion.municipio = municipio.ToString().Trim();
            direccion.complemento = complemento.ToString().Trim();
         

            return direccion;
        }

        private direccion adddireccionreceptor(string departamento, string municipio, string complemento)
        {
            direccion direccion = new direccion();
            direccion.departamento = departamento.ToString().Trim();
            direccion.municipio = municipio.ToString().Trim();
            direccion.complemento = complemento.ToString().Trim();


            return direccion;
        }

        private CuerpoDocumento adddetalle(int numItem, int tipoItem, string numeroDocumento, string codigo, string codTributo, string descripcion, double cantidad, int uniMedida, double precioUni, double montoDescu, double ventaNoSuj, double ventaExenta, double ventaGravada, List<string> tributos, double psv, double noGravado,double ivaItem)
        {
           CuerpoDocumento detalle = new CuerpoDocumento();

            detalle.numItem = numItem;
            detalle.tipoItem = tipoItem;
            detalle.numeroDocumento= numeroDocumento;
            detalle.codigo= codigo;
            detalle.codTributo= codTributo;
            detalle.descripcion = descripcion;
            detalle.cantidad=cantidad;
            detalle.uniMedida= uniMedida;
            detalle.precioUni= precioUni;
            detalle.tributos = tributos;
            detalle.ventaGravada = ventaGravada;
            detalle.montoDescu= montoDescu;
            detalle.ivaItem= ivaItem;
            detalle.noGravado = noGravado;
            detalle.ventaExenta = ventaExenta;
            detalle.ventaNoSuj = ventaNoSuj;


            return detalle;
        }

        private Tributo addtributos(string codigo, string descripcion,double valor)
        {
            Tributo  tr = new Tributo();
            tr.codigo = codigo.ToString().Trim();
            tr.descripcion = descripcion.ToString().Trim();
            tr.valor = valor;

            return tr;
        }

        private DocumentoRelacionado addDocumentosRelacionados(string tipoDocumento, int tipoGeneracion, string numeroDocumento, string fechaEmision)
        {
            DocumentoRelacionado dr = new DocumentoRelacionado();
            dr.tipoDocumento = tipoDocumento.ToString().Trim();
            dr.tipoGeneracion = tipoGeneracion;
            dr.numeroDocumento = numeroDocumento.ToString().Trim();
            dr.fechaEmision = fechaEmision.ToString().Trim();
            return dr;
        }

        private Extension addExtension(string nombEntrega, string docuEntrega, string nombRecibe, string docuRecibe, string observaciones, string placaVehiculo)
        {
            Extension extension = new Extension();
            extension.nombEntrega = nombEntrega;
            extension.docuEntrega = docuEntrega;
            extension.docuRecibe = docuRecibe;
            extension.nombRecibe = nombRecibe;
            extension.observaciones = observaciones;
            extension.placaVehiculo = placaVehiculo;
          
          
            return extension;
        }

        private Apendice addApendice(string campo, string etiqueta, string valor)
        {
            Apendice apendice = new Apendice();
            
            apendice.campo = campo;
            apendice.etiqueta = etiqueta;
            apendice.valor= valor;


            return apendice;
        }


        

    }
}
