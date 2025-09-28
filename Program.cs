namespace RentalCar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Allow",
                    policy => policy.WithOrigins("http://localhost:5173") 
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("Allow"); 
            
            app.MapControllers();

            app.Run();
        }
    }
}
