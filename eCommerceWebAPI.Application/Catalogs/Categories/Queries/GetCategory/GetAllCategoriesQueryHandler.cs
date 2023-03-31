using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Application.Configuration.Queries;
using Dapper;
using eCommerceWebAPI.Domain.Catalogs;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Queries.GetCategory
{
    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, IList<CategoryDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlRequestedCategories = $@"
                                    SELECT DISTINCT
                                       [Category].[Id],
                                       [Category].[Name],
                                       [Category].[Description],
                                       [Category].[DisplayOrder],
                                       [Category].[ParentCategoryId]
                                   FROM [Category]
                                   WHERE [Category].ParentCategoryId IS NULL OR [Category].ParentCategoryId <= 0
                                   ORDER BY [Category].DisplayOrder
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var requestedCategoriesEnumerable = await connection.QueryAsync<Category>(sqlRequestedCategories, new { request.Skip, request.PageSize });
                var requestedCategoriesList = requestedCategoriesEnumerable.ToList();

                const string sqlCategories = $@"
                                    SELECT DISTINCT
                                       [Category].[Id],
                                       [Category].[Name],
                                       [Category].[Description],
                                       [Category].[DisplayOrder],
                                       [Category].[ParentCategoryId]
                                   FROM [Category]";
                var categoriesEnumerable = await connection.QueryAsync<Category>(sqlCategories);
                var categoriesList = categoriesEnumerable.ToList();

                var allTempCategories = categoriesList;
                List<Category> resultCategories = new();

                do
                {
                    var allTempCategoriesIdsList = allTempCategories.Select(c => c.Id).ToList();
                    var allTempCategoriesParentCategoyIdsList = allTempCategories.Where(c => c.ParentCategoryId != null).Select(c => c.ParentCategoryId).Distinct().ToList();

                    var lastChildCategories = allTempCategories.Where(c => !allTempCategoriesParentCategoyIdsList.Contains(c.Id)).ToList();
                    var lastChildCategoriesIds = lastChildCategories.Select(c => c.Id).ToList();
                    var lastChildCategoriesParentCategoryIds = lastChildCategories.Where(c => c.ParentCategoryId != null).Select(c => c.ParentCategoryId).DistinctBy(x => x).ToList();

                    List<Category> allAddedLastChildCategories = new();
                    var lastChildCategoriesParentCategories = allTempCategories.Where(c => lastChildCategoriesParentCategoryIds.Contains(c.Id)).ToList();
                    foreach (var lastChildCategoriesParentCategory in lastChildCategoriesParentCategories)
                    {
                        var relatedLastChildCategories = lastChildCategories
                            .Where(c => c.ParentCategoryId != null && c.ParentCategoryId == lastChildCategoriesParentCategory.Id)
                            .OrderBy(c => c.DisplayOrder)
                            .ToList();
                        relatedLastChildCategories.ForEach(lastChildCategoriesParentCategory.ChildCategories.Add);

                        allTempCategories
                            .Where(c => c.Id == lastChildCategoriesParentCategory.Id)
                            .Select(c => c = lastChildCategoriesParentCategory)
                            .ToList();

                        allAddedLastChildCategories.AddRange(relatedLastChildCategories);
                    }

                    var allAddedLastChildCategoriesIds = allAddedLastChildCategories.Select(c => c.Id).DistinctBy(x => x).ToList();
                    var allNotAddedLastChildCategories = lastChildCategories.Where(c => !allAddedLastChildCategoriesIds.Contains(c.Id)).ToList();
                    var allNotAddedLastChildCategoriesIds = allNotAddedLastChildCategories.Select(c => c.Id).DistinctBy(x => x).ToList();

                    var substitutionAllTempCategories = allTempCategories
                        .Where(c =>
                            !lastChildCategoriesIds.Contains(c.Id) ||
                            (allAddedLastChildCategoriesIds.Count > 0 && allNotAddedLastChildCategoriesIds.Contains(c.Id)))
                        .ToList();
                    if (!substitutionAllTempCategories.Any())
                    {
                        resultCategories.AddRange(lastChildCategories);
                    }

                    allTempCategories = substitutionAllTempCategories
                        .OrderBy(x => x.DisplayOrder)
                        .ToList();
                }
                while (allTempCategories.Count > 0);

                var requestedCategoriesIdsList = requestedCategoriesList.Select(x => x.Id).ToList();
                resultCategories = resultCategories
                    .Where(c => requestedCategoriesIdsList.Contains(c.Id))
                    .OrderBy(c => c.Id)
                    .ToList();

                var categoryDtos = _mapper.Map<List<CategoryDto>>(resultCategories);
                return categoryDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
