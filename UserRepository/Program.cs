﻿using Domain;
using Domain.packages;
using Domain.packages.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserRepository.Repository;
using UserRepository.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddDbContext<UserRepositoryContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("UserDbConnectionString"), new MySqlServerVersion(new Version(5, 7))));
builder.Services.AddDbContext<UserRepositoryContext>(options =>
    options.UseSqlite("Data source=./db.db"));
builder.Services.AddScoped<IUserRepository, UserRepository.Repository.UserRepository>(); // Assuming UserRepository implements IUserRepository
builder.Services.AddScoped<UserRepositoryService>();
builder.Services.AddScoped<UserRepositoryHandlers>();
builder.Services.AddHostedService<RpcBackgroundService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();



app.Run();


