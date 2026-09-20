using System.Linq.Expressions;

namespace Delivery.Core.Interfaces;

public interface IRepositoryGeneric<T> where T : class
{
    /// <summary>
    /// Obtiene informacion por Id
    /// </summary>
    /// <param name="id">Ide del objeto a buscar.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    /// <returns>Objeto de la base de datos.</returns>
    Task<T?> GetByIdAsync(object id, bool disableTracking = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una lista de todos los objetos de la base de datos.
    /// </summary>
    /// <param name="disableTracking">Habilita la modificacion de datos.</param>
    /// <param name="cancellationToken">Token de cancelacion</param>
    /// <returns>Lista de objetos</returns>
    Task<IEnumerable<T>> GetAllAsync(bool disableTracking = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una lista de objetos que cumplan con la condición.
    /// </summary>
    /// <param name="predicate">Condicion o parametros a definir.</param>
    /// <param name="disableTracking">Habilita la modificacion de datos.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    /// <returns>Lista de objetos filtrada.</returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, bool disableTracking = false, CancellationToken cancellationToken = default, 
    params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Verifica si existe algún objeto que cumpla con la condición especificada.
    /// </summary>
    /// <param name="predicate">Condición o parametro a definir.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Regresa true si existe al menos un objeto que cumpla con la condición, false en caso contrario.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el primer objeto que cumpla con la condición especificada o null si no existe ninguno.
    /// </summary>
    /// <param name="predicate">Condición o parametros a definir.</param>
    /// <param name="disableTracking">Habilita la modificacion de datos.</param>
    /// <param name="cancellationToken">Token de cancelacion</param>
    /// <param name="includes">Condicion de Join</param>
    /// <returns>Regresa el primer objeto que cumpla con la condición o null si no existe ninguno.</returns>
    Task<T?>  FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool disableTracking, CancellationToken cancellationToken = default, 
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Agrega una entidad a la base de datos.
    /// </summary>
    /// <param name="entity">Información a agregar.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una entidad.
    /// </summary>
    /// <param name="entity">Datos de la entidad actualizada.</param>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    /// <param name="entity">Entidad a elminar.</param>
    void Delete(T entity);

    /// <summary>
    /// Agrega varias entidades al contexto de persistencia de forma asíncrona.
    /// </summary>
    /// <param name="entities">Entidades que se agregarán.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza varias entidades en el contexto de persistencia.
    /// </summary>
    /// <param name="entities">Entidades que se actualizarán.</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Elimina varias entidades del contexto de persistencia.
    /// </summary>
    /// <param name="entities">Entidades que se eliminarán.</param>
    void DeleteRange(IEnumerable<T> entities);
}