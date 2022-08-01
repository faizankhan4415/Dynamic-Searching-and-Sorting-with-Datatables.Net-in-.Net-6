

        /// <summary>
        /// Dynamic Where Clause with multiple search criterias
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>

public static IQueryable<T> WhereD<T>(
            this IQueryable<T> source, List<SearchCriteria> searchCriteria)
        {


            foreach (var item in searchCriteria)
            {
                string searchValue = item.SearchValue;
                string propertyName = item.SearchBy;


                if (string.IsNullOrEmpty(searchValue))
                {
                    return source;
                }


                try
                {

                    PropertyInfo prop = null;


                    prop = typeof(T).GetProperty(propertyName);

                    if (prop != null)
                    {

                        var propertyType = prop.PropertyType;

                        ParameterExpression param = Expression.Parameter(typeof(T));
                        Expression propertyExp = Expression.Property(param, prop);
                        if (propertyType == typeof(string))
                        {
                            
                            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            Expression codeVal = Expression.Constant(searchValue, typeof(string));
                            Expression containsMethodExp = Expression.Call(propertyExp, method, codeVal);
                            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(containsMethodExp, param);
                            source = source.Where(predicate);

                        }
                        else if (propertyType == typeof(int))
                        {

                            Expression codeVal = Expression.Constant(Convert.ToInt32(searchValue),typeof(int));
                            Expression body = Expression.Equal(propertyExp, codeVal);
                            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(body, param);
                            source = source.Where(predicate);

                        }
                        else if (propertyType == typeof(bool))
                        {
                            Expression codeVal = Expression.Constant(searchValue, typeof(bool));
                            Expression body = Expression.Equal(propertyExp, codeVal);
                            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(body, param);
                            source = source.Where(predicate);

                        }


                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


            }



            
            return source;
        }




        /// <summary>
        /// Dynamic Order By Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderByProperty"></param>
        /// <param name="direction"></param>
        /// <returns></returns>


public static IQueryable<T> OrderByQuery<T>(this IQueryable<T> source,
                                                    string orderByProperty, DtOrderDir direction)
        {
            if (String.IsNullOrEmpty(orderByProperty))
            {
                return source;
            }

            string methodName = direction == DtOrderDir.Desc ? "OrderByDescending" : "OrderBy";

            ParameterExpression param = Expression.Parameter(source.ElementType,"p");

            MemberExpression prop = Expression.Property(param, orderByProperty);
            LambdaExpression lambda = Expression.Lambda(prop, param);

            
            Expression resultExpression = Expression.Call(typeof(Queryable), methodName,
                                  new Type[] { source.ElementType, prop.Type },
                                  source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(resultExpression);


           }



        /// <summary>
        /// Dynamic Filter Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>



public static IQueryable<T> FilterQuery<T>(this IQueryable<T> source, int start,int length)
        {            
            source = source.Skip(start).Take(length);

            return source;
        }


