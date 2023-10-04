using Microsoft.EntityFrameworkCore;

namespace MovieDatabase.Data;

public partial class MovieLensContext : DbContext
{
    public MovieLensContext()
    {
    }

    public MovieLensContext(DbContextOptions<MovieLensContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Link> Links { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MoviesGenre> MoviesGenres { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>(entity =>
        {
            entity.Property(e => e.MovieId).ValueGeneratedNever();

            entity.HasOne(d => d.Movie).WithOne(p => p.Link)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesLinks");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(e => e.MovieId).ValueGeneratedNever();
        });

        modelBuilder.Entity<MoviesGenre>(entity =>
        {
            entity.HasOne(d => d.Genre).WithMany(p => p.MoviesGenres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Genres");

            entity.HasOne(d => d.Movie).WithMany(p => p.MoviesGenres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movies");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasOne(d => d.Movie).WithMany(p => p.Ratings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesRatings");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasOne(d => d.Movie).WithMany(p => p.Tags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoviesTags");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
