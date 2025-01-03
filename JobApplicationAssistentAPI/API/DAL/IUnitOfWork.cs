﻿using API.DAL.Models;
using API.DAL.Repositories;

namespace API.DAL
{
    public interface IUnitOfWork
    {
        IRepository<Job> JobRepository { get; }
        IRepository<Company> CompanyRepository { get; }
        IRepository<Application> ApplicationRepository { get; }
        Task SaveAsync();
    }
}
