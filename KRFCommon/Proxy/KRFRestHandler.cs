namespace KRFCommon.Proxy
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using KRFCommon.Constants;
    using KRFCommon.CQRS.Common;
    using KRFCommon.JSON;

    public static class KRFRestHandler
    {
        public static async Task<KRFHttpResponse<TResp>> RequestHttp<TBody, TResp>( KRFHttpRequest request )
        {
            return await RequestHttp<bool, TResp>( request );
        }

        public static async Task<KRFHttpResponse<TResp>> RequestHttp<TBody, TResp>( KRFHttpRequestWithBody<TBody> request )
            where TBody : class
            where TResp : class
        {
            if ( request == null )
            {
                throw new Exception( "No request was defined for Http Rest call" );
            }

            HttpResponseMessage response = null;
            string route = string.Format( "{0}{1}", request.Route, request.QueryString );
            try
            {
                using ( var handler = new HttpClientHandler() )
                {
                    if ( request.ForceDisableSSL.HasValue && request.ForceDisableSSL.Value )
                    {
                        handler.ServerCertificateCustomValidationCallback = ( message, cert, chain, err ) =>
                        {
                            return true;
                        };
                    }
                    else
                    {
                        if ( !string.IsNullOrEmpty( request.CertificatePath ) )
                        {
                            //Add ssl certificate
                            if ( !string.IsNullOrEmpty( request.CertificateKey ) )
                            {
                                //Add ssl certificate password
                            }
                        }
                    }

                    using ( var client = new HttpClient( handler ) )
                    {
                        client.BaseAddress = new Uri( request.Url );
                        client.DefaultRequestHeaders.Accept.Append( new MediaTypeWithQualityHeaderValue( KRFConstants.JsonContentType ) );

                        if ( !string.IsNullOrEmpty( request.KRFBearerToken ) && !string.IsNullOrEmpty( request.KRFBearerTokenHeader ) )
                        {
                            client.DefaultRequestHeaders.Add( request.KRFBearerTokenHeader, request.KRFBearerToken );
                        }

                        if ( request.Timeout.HasValue )
                        {
                            client.Timeout = new TimeSpan( 0, 0, request.Timeout.Value );
                        }

                        switch ( request.Method )
                        {
                            case HttpMethodEnum.GET:
                            {
                                response = await client.GetAsync( route );
                                break;
                            }
                            case HttpMethodEnum.DELETE:
                            {
                                response = await client.DeleteAsync( route );
                                break;
                            }
                            case HttpMethodEnum.POST:
                            case HttpMethodEnum.PUT:
                            {
                                string stringBody = string.Empty;
                                if ( request.Body != null )
                                {
                                    stringBody = request.Body is string ? ( request.Body as string ) : JsonSerializer.Serialize( request.Body, KRFJsonSerializerOptions.GetJsonSerializerOptions() );
                                }

                                using ( HttpContent req = new StringContent( stringBody, Encoding.UTF8 ) )
                                {
                                    req.Headers.ContentType.MediaType = KRFConstants.JsonContentType;
                                    req.Headers.ContentType.CharSet = "utf-8";

                                    if ( request.Method.Equals( HttpMethodEnum.POST ) )
                                    {
                                        response = await client.PostAsync( route, req );
                                    }
                                    else
                                    {
                                        response = await client.PutAsync( route, req );
                                    }
                                }
                                break;
                            }
                        }

                        if ( response == null || response.Content == null )
                        {
                            return new KRFHttpResponse<TResp>
                            {
                                Error = new ErrorOut( HttpStatusCode.InternalServerError, string.Format( "Could not retrieve response from {0}/{1}", request.Url, route ) ),
                                HttpStatus = HttpStatusCode.InternalServerError
                            };
                        }


                        var respBody = await response.Content.ReadAsStringAsync();
                        if ( response.StatusCode == HttpStatusCode.OK )
                        {
                            return new KRFHttpResponse<TResp>
                            {
                                Response = JsonSerializer.Deserialize<TResp>( respBody, KRFJsonSerializerOptions.GetJsonSerializerOptions() ),
                                HttpStatus = response.StatusCode,
                                ResponseHeaders = response.Headers
                            };
                        }
                        else
                        {
                            return new KRFHttpResponse<TResp>
                            {
                                Error = JsonSerializer.Deserialize<ErrorOut>( respBody, KRFJsonSerializerOptions.GetJsonSerializerOptions() ),
                                HttpStatus = response.StatusCode,
                                ResponseHeaders = response.Headers
                            };
                        }
                    }
                }
            }
            catch
            {
                return new KRFHttpResponse<TResp>
                {
                    Error = new ErrorOut( HttpStatusCode.BadRequest, string.Format( "Could not retrieve response from {0}/{1}", request.Url, route ) ),
                    HttpStatus = HttpStatusCode.BadRequest
                };
            }
        }
    }
}
