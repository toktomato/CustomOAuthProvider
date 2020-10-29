using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OAP.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OAP.Controllers
{
    [Route("/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        IConfiguration config;
        public AuthenticationController(IConfiguration configuration)
        {
            config = configuration;
        }

        public string GenerateToken(Roles roles)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtToken:SecretKey"]));
            var creds = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256);
            var issuer = config["JwtToken:Issuer"];
            var audience = config["JwtToken:Audience"];
            var jwtValidity = DateTime.Now.AddMinutes(Convert.ToDouble(config["JwtToken:TokenExpiry"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Issuer = issuer,
                IssuedAt = DateTime.Now,
                Expires = jwtValidity,
                NotBefore = DateTime.Now,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("adpt", roles.AdminDept.ToString())
                }),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost]
        public ActionResult Login([FromBody]JsonElement jsondata)
        {
            string json = jsondata.ToString();
            AuthOutgoingDataModel authReturn = new AuthOutgoingDataModel();

            AuthIncomingDataModel data = JsonConvert.DeserializeObject<AuthIncomingDataModel>(json);
            string connstring = config["SQLConnString"];

            SqlConnection con = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand("select * from Users where Usr=@username and pwd=@password", con);
            cmd.Parameters.Add("@username", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@username"].Value = data.Username;
            cmd.Parameters.Add("@password", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@password"].Value = data.Password;

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader != null)
                {
                    Roles roles = new Roles();
                    while (reader.Read())
                    {
                        IDataRecord record = (IDataReader)reader;
                        roles = new Roles()
                        {
                            AdminDept = record[5].ToString()
                        };
                    }
                    authReturn.IDToken = GenerateToken(roles);
                }
                else
                {
                    return new UnprocessableEntityResult();
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.StackTrace.ToString());
            }
            return Ok(authReturn);
        }
    }
}
