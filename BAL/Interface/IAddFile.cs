using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IAddFile
    {
        public void AddFile(IFormFile file, String path,string filename);

        public void RemoveFile(string path);
    }
}
