﻿using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JikanDotNet.Tests
{
	public class MangaSearchTestClass
	{
		private readonly IJikan _jikan;

		public MangaSearchTestClass()
		{
			_jikan = new Jikan();
		}

		[Theory]
		[InlineData("berserk")]
		[InlineData("monster")]
		[InlineData("death")]
		public async Task SearchManga_NonEmptyQuery_ShouldReturnNotNullSearchManga(string query)
		{
			// When
			var returnedManga = await _jikan.SearchManga(query);

			// Then
			returnedManga.Should().NotBeNull();
		}

		[Fact]
		public async Task SearchManga_DanganronpaQuery_ShouldReturnDanganronpaManga()
		{
			// When
			var danganronpaManga = await _jikan.SearchManga("danganronpa");

			danganronpaManga.ResultLastPage.Should().Be(20);
		}

		[Fact]
		public async Task SearchManga_YotsubatoQuery_ShouldReturnYotsubatoManga()
		{
			// When
			var yotsubato = await _jikan.SearchManga("yotsubato");

			// Then
			var firstResult = yotsubato.Results.First();
			using (new AssertionScope())
			{
				firstResult.Title.Should().Be("Yotsuba to!");
				firstResult.Type.Should().Be("Manga");
				firstResult.Volumes.Should().Be(0);
				firstResult.MalId.Should().Be(104);
			}
		}

		[Fact]
		public async Task SearchManga_YotsubatoPublishingQuery_ShouldReturnPublishedYotsubatoManga()
		{
			// Given
			var searchConfig = new MangaSearchConfig()
			{
				Status = AiringStatus.Airing
			};

			// When
			var yotsubato = await _jikan.SearchManga("yotsubato", searchConfig);

			// Then
			var firstResult = yotsubato.Results.First();
			using (new AssertionScope())
			{
				firstResult.Title.Should().Be("Yotsuba to!");
				firstResult.Type.Should().Be("Manga");
				firstResult.Volumes.Should().Be(0);
				firstResult.MalId.Should().Be(104);
			}
		}

		[Fact]
		public async Task SearchManga_GirlQuerySecondPage_ShouldFindGirlManga()
		{
			// When
			var returnedAnime = await _jikan.SearchManga("girl", 2);

			// Then
			using (new AssertionScope())
			{
				returnedAnime.Results.Select(x => x.Title).Should().Contain("Tokyo Boys & Girls");
				returnedAnime.ResultLastPage.Should().Be(20);
			}
		}

		[Theory]
		[InlineData("berserk")]
		[InlineData("monster")]
		[InlineData("death")]
		public async Task SearchManga_MangaConfig_ShouldReturnNotNullSearchManga(string query)
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga(query, searchConfig);

			// Then
			returnedManga.Should().NotBeNull();
		}

		[Fact]
		public async Task SearchManga_DanganronpaMangaConfig_ShouldReturnDanganronpaManga()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Type = MangaType.Manga
			};

			// When
			var danganronpaManga = await _jikan.SearchManga("danganronpa", searchConfig);

			// Then
			danganronpaManga.ResultLastPage.Should().Be(4);
		}

		[Fact]
		public async Task SearchManga_DanganronpaMangaAbove7Config_ShouldReturnDanganronpaMangaScore()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Type = MangaType.Manga,
				Score = 7
			};

			// When
			var danganronpaManga = await _jikan.SearchManga("danganronpa", searchConfig);

			danganronpaManga.Results.First().Title.Should().Contain("Dangan");
		}

		[Fact]
		public async Task SearchManga_MangaGameGenreConfig_ShouldFilterMetalFightBeyblade()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Type = MangaType.Manga,
			};
			searchConfig.Genres.Add(GenreSearch.Game);

			// When
			var returnedManga = await _jikan.SearchManga("metal", searchConfig);

			returnedManga.Results.First().Title.Should().Be("Metal Fight Beyblade");
		}

		[Fact]
		public async Task SearchManga_MetalBefore2014Config_ShouldFilterFMPEndDate()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				EndDate = new DateTime(2015, 1, 1)
			};

			// When
			var returnedManga = await _jikan.SearchManga("metal", searchConfig);

			// Then
			var titles = returnedManga.Results.Select(x => x.Title);
			using (new AssertionScope())
			{
				titles.Should().Contain("Fullmetal Alchemist");
				titles.Should().Contain("Full Metal Panic!");
			}
		}

		[Fact]
		public async Task SearchManga_MetalSortByMembersConfig_ShouldSortByPopularityFairyTailFirst()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				OrderBy = MangaSearchSortable.Members,
				SortDirection = SortDirection.Descending
			};

			// When
			var returnedManga = await _jikan.SearchManga("metal", searchConfig);

			// Then
			var titles = returnedManga.Results.Select(x => x.Title);
			using (new AssertionScope())
			{
				titles.Should().Contain("Fullmetal Alchemist");
				titles.Should().Contain("Fairy Tail");
				returnedManga.Results.First().Title.Should().Be("Fairy Tail");
			}
		}

		[Fact]
		public async Task SearchManga_OneSortByIdConfig_ShouldSortByIdOnePieceFirst()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				OrderBy = MangaSearchSortable.Id,
				SortDirection = SortDirection.Ascending
			};

			// When
			var returnedManga = await _jikan.SearchManga("one", searchConfig);

			// Then
			returnedManga.Results.First().Title.Should().Be("One Piece");
		}

		[Fact]
		public async Task SearchManga_TorikoShonenJumpMagazineConfig_ShouldReturnTwoEntries()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				MagazineId = 83
			};

			// When
			var returnedManga = await _jikan.SearchManga("toriko", searchConfig);

			// Then
			using (new AssertionScope())
			{
				returnedManga.Results.First().Title.Should().Be("Toriko");
				returnedManga.Results.Should().HaveCount(3);
			}
		}

		[Fact]
		public async Task SearchManga_TorikoIncorrectMagazineConfig_ShouldNotFilter()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				MagazineId = -1
			};

			// When
			var returnedManga = await _jikan.SearchManga("toriko", searchConfig);

			// Then
			using (new AssertionScope())
			{
				returnedManga.Results.First().Title.Should().Be("Toriko");
				returnedManga.Results.Count.Should().BeGreaterThan(30);
			}
		}

		[Fact]
		public async Task SearchManga_EmptyQueryActionManga_ShouldFindCrowAnd007()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Genres = new List<GenreSearch> { GenreSearch.Action },
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga(searchConfig);

			// Then
			var titles = returnedManga.Results.Select(x => x.Title);
			using (new AssertionScope())
			{
				titles.Should().Contain("-Crow-");
				titles.Should().Contain("007 Series");
				returnedManga.Results.Count.Should().BeGreaterThan(30);
			}
		}

		[Fact]
		public async Task SearchManga_EmptyQueryActionMangaFirstPage_ShouldFindCrowAnd007()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Genres = new List<GenreSearch> { GenreSearch.Action },
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga(searchConfig, 1);

			// Then
			var titles = returnedManga.Results.Select(x => x.Title);
			using (new AssertionScope())
			{
				titles.Should().Contain("-Crow-");
				titles.Should().Contain("007 Series");
				returnedManga.Results.Count.Should().BeGreaterThan(30);
			}
		}

		[Fact]
		public async Task SearchManga_EmptyQueryActionMangaSecondPage_ShouldFind888AndAccelWorld()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Genres = new List<GenreSearch> { GenreSearch.Action },
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga(searchConfig, 2);

			// Then
			var titles = returnedManga.Results.Select(x => x.Title);
			using (new AssertionScope())
			{
				titles.Should().Contain("888");
				titles.Should().Contain("Accel World");
				returnedManga.Results.Count.Should().BeGreaterThan(30);
			}
		}

		[Fact]
		public async Task SearchManga_OreQueryComedyMangaSecondPage_ShouldReturnNotEmptyCollection()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Genres = new List<GenreSearch> { GenreSearch.Comedy },
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga("ore", 2, searchConfig);

			// Then
			using (new AssertionScope())
			{
				returnedManga.Should().NotBeNull();
				returnedManga.Results.Should().NotBeNullOrEmpty();
			}
		}

		[Fact]
		public async Task SearchManga_GenreInclusion_ShouldReturnNotEmptyCollection()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Genres = new List<GenreSearch> { GenreSearch.Comedy },
				GenreIncluded = true,
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga(searchConfig);

			// Then
			using (new AssertionScope())
			{
				returnedManga.Should().NotBeNull();
				returnedManga.Results.Should().NotBeNullOrEmpty();
			}
		}

		[Fact]
		public async Task SearchManga_GenreExclusion_ShouldReturnNotEmptyCollection()
		{
			// Given
			var searchConfig = new MangaSearchConfig
			{
				Genres = new List<GenreSearch> { GenreSearch.Comedy },
				GenreIncluded = false,
				Type = MangaType.Manga
			};

			// When
			var returnedManga = await _jikan.SearchManga(searchConfig);

			// Then
			using (new AssertionScope())
			{
				returnedManga.Should().NotBeNull();
				returnedManga.Results.Should().NotBeNullOrEmpty();
			}
		}
	}
}
