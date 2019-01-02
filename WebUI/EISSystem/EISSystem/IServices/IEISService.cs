using System.Net.Http;

namespace EIS.WebApp.IServices
{
    public interface IEISService<T>
    {
        HttpClient GetService();
        HttpResponseMessage GetResponse(string url);
        HttpResponseMessage PostResponse(string url,T entity);
        HttpResponseMessage PutResponse(string url, T entity);

        HttpResponseMessage DeleteResponse(string url);
    }
}
