using Acme.BookStore.Authors;
using Acme.BookStore.Permissions;
using Blazorise;
using Blazorise.DataGrid;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectMapping;

namespace Acme.BookStore.Blazor.Pages
{
    /// <summary>
    /// 该文件的建立：直接新建一个与Authors.razor同名的class，如果需要加入css文件，直接加入同名的css即可
    /// 该文件类似于zaror组件里的@{}的C#代码
    /// </summary>
    public partial class Authors
    {
        private IReadOnlyList<AuthorDto> AuthorList { get; set; }

        //分页信息
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }

        //权限信息
        private bool CanCreateAuthor { get; set; }
        private bool CanEditAuthor { get; set; }
        private bool CanDeleteAuthor { get; set; }

        //新建作者
        private CreateAuthorDto NewAuthor { get; set; }

        //编辑作者信息
        private Guid EditingAuthorId { get; set; }
        private UpdateAuthorDto EditingAuthor { get; set; }

        //模态框
        private Modal CreateAuthorModal { get; set; }
        private Modal EditAuthorModal { get; set; }

        //验证
        private Validations CreateValidationsRef;
        private Validations EditValidationsRef;

        public Authors()
        {
            NewAuthor = new CreateAuthorDto();
            EditingAuthor = new UpdateAuthorDto();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetAuthorsAsync();
        }

        /// <summary>
        /// 获取权限
        /// </summary>
        /// <returns></returns>
        private async Task SetPermissionsAsync()
        {
            //AuthorizationService 在组件页面 注入了 IAuthorAppService
            CanCreateAuthor = await AuthorizationService.IsGrantedAsync(BookStorePermissions.Authors.Create);
            CanEditAuthor = await AuthorizationService.IsGrantedAnyAsync(BookStorePermissions.Authors.Edit);
            CanDeleteAuthor = await AuthorizationService.IsGrantedAnyAsync(BookStorePermissions.Authors.Delete);
        }

        /// <summary>
        /// 获取Authors列表
        /// </summary>
        /// <returns></returns>
        private async Task GetAuthorsAsync()
        {
            var result = await AuthorAppService.GetListAsync(
                    new GetAuthorListDto
                    {
                        MaxResultCount = PageSize,
                        SkipCount = CurrentPage * PageSize,
                        Sorting = CurrentSorting
                    }
                );

            AuthorList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AuthorDto> e)
        {
            CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? "DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetAuthorsAsync();

            // 通知组件已经更改, 让组件重新渲染
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// 打开新建模态框
        /// </summary>
        private void OpenCreateAuthorModal()
        {
            //清除字段信息
            CreateValidationsRef.ClearAll();

            NewAuthor = new CreateAuthorDto();
            CreateAuthorModal.Show();
        }

        /// <summary>
        /// 关闭新建模态框
        /// </summary>
        private void CloseCreateAuthorModal()
        {
            CreateAuthorModal.Hide();
        }

        /// <summary>
        /// 保存新建
        /// </summary>
        /// <returns></returns>
        private async Task CreateAuthorAsync()
        {
            if (await CreateValidationsRef.ValidateAll())
            {
                await AuthorAppService.CreateAsync(NewAuthor);
                await GetAuthorsAsync();
                CreateAuthorModal.Hide();
            }
        }

        /// <summary>
        /// 打开编辑模态框
        /// </summary>
        /// <param name="author"></param>
        private void OpenEditAuthorModal(AuthorDto author)
        {
            EditValidationsRef.ClearAll();

            EditingAuthorId = author.Id;
            //映射实体，需要在 Blazor BookStoreBlazorAutoMapperProfile的构造函数增加一个映射
            EditingAuthor = ObjectMapper.Map<AuthorDto, UpdateAuthorDto>(author);
            EditAuthorModal.Show();
        }

        /// <summary>
        /// 关闭编辑模态框
        /// </summary>
        private void CloseEditAuthorModal()
        {
            EditAuthorModal.Hide();
        }

        /// <summary>
        /// 保存编辑
        /// </summary>
        /// <returns></returns>
        private async Task UpdateAuthorAsync()
        {
            if (await EditValidationsRef.ValidateAll())
            {
                await AuthorAppService.UpdateAsync(EditingAuthorId, EditingAuthor);
                await GetAuthorsAsync();
                EditAuthorModal.Hide();
            }
        }

        /// <summary>
        /// 删除作者
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        private async Task DeleteAuthorAsync(AuthorDto author)
        {
            var confirmMessage = L["AuthorDeletionConfirmationMessage", author.Name];
            if (!await Message.Confirm(confirmMessage))
            {
                return;
            }

            await AuthorAppService.DeleteAsync(author.Id);
            await GetAuthorsAsync();
        }
    }
}
