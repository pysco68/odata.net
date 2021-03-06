﻿//---------------------------------------------------------------------
// <copyright file="SelectExpandBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Query.TDD.Tests.UriBuilder
{
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriBuilder;
    using Microsoft.OData.Core.UriParser;
    using System.Collections.Generic;

    [TestClass]
    public class SelectExpandBuilderTests
    {
        protected static Uri ServiceRoot = new Uri("http://gobbledygook/");
        protected readonly ODataUriParserSettings settings = new ODataUriParserSettings();

        #region $select with no $expand
        [TestMethod]
        public void SelectSingleDeclaredPropertySucceeds()
        {
            Uri queryUri = new Uri("People?$select=Name", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=Name"), actualUri);
        }

        [TestMethod]
        public void SelectWithEmptyStringMeansEverything()
        {
            Uri queryUri = new Uri("People?$select=", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People"), actualUri);
        }

        [TestMethod]
        public void SelectWithNoStringMeansNothing()
        {
            Uri queryUri = new Uri("People?", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People"), actualUri);
        }

        [TestMethod]
        public void WildcardPreemptsAllStructuralProperties()
        {
            Uri queryUri = new Uri("People?$select=Name, *, MyAddress", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=*"), actualUri);
        }

        [TestMethod]
        public void SelectEnumStructuralProperty()
        {
            Uri queryUri = new Uri("Pet2Set?$select=PetColorPattern", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet2Set?$select=PetColorPattern"), actualUri);
        }

        [TestMethod]
        public void SelectEnumStructuralPropertyWildcard()
        {
            Uri queryUri = new Uri("Pet2Set?$select=PetColorPattern,*", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet2Set?$select=*"), actualUri);
        }

        [TestMethod]
        public void SelectNavigationPropertyWithoutExpandMeansSelectLink()
        {
            Uri queryUri = new Uri("People?$select=MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=MyDog"), actualUri);
        }

        [TestMethod]
        public void SelectActionMeansOperation()
        {
            Uri queryUri = new Uri("Dogs?$select=Fully.Qualified.Namespace.Walk", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs?$select=Fully.Qualified.Namespace.Walk"), actualUri);
        }

        [TestMethod]
        public void SelectWorksWithEntitySet()
        {
            Uri queryUri = new Uri("People?$select=Name", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=Name"), actualUri);
        }

        [TestMethod]
        public void MultipleSelectionsWorkWithEntitySet()
        {
            Uri queryUri = new Uri("People?$select=Name, MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyDog"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectSupportsTypeSegments()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Employee/PaintingsInOffice", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/PaintingsInOffice"), actualUri.OriginalString);
        }

        [TestMethod]
        public void UnneededTypeSegmentInSelectIsOk()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Employee/Name", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/Name"), actualUri.OriginalString);
        }

        [TestMethod]
        public void TypeSegmentForVeryDerivedTypeAndSelectPropertyOfMiddleDerivedType()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Manager/WorkEmail", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/WorkEmail"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectNavigationPropertyOnDerivedType()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Manager/PaintingsInOffice", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/PaintingsInOffice"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectOpenPropertyOnDerivedType()
        {
            Uri queryUri = new Uri("Paintings?$select=Fully.Qualified.Namespace.FramedPainting/OpenProp", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Paintings?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.FramedPainting/OpenProp"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectOpenPropertyOnDerivedTypeWhereBaseTypeIsNotOpen()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.OpenEmployee/OpenProp", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.OpenEmployee/OpenProp"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectFunctionWithOverloadsScopedByTypeSegment()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectActionWithOverloads()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Move", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=Fully.Qualified.Namespace.Move"), actualUri);
        }

        [TestMethod]
        public void SelectActionWithOverloadsScopedByTypeSegment()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move"), actualUri.OriginalString);
        }

        [TestMethod]
        public void NamespaceQualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOperation()
        {
            Uri queryUri = new Uri("Paintings?$select=Fully.Qualified.Namespace.Restore", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings?$select=Fully.Qualified.Namespace.Restore"), actualUri);
        }

        [TestMethod]
        public void CanSelectSubPropertyOfComplexType()
        {
            Uri queryUri = new Uri("People?$select=MyAddress/City", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("MyAddress/City"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectManyDeclaredPropertiesSucceeds()
        {
            Uri queryUri = new Uri("People?$select= Shoe, Birthdate,GeographyPoint,    TimeEmployed, \tPreviousAddresses", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Shoe,Birthdate,GeographyPoint,TimeEmployed,PreviousAddresses"), actualUri.OriginalString);
        }


        [TestMethod]
        public void SelectOpenPropertySucceeds()
        {
            Uri queryUri = new Uri("Paintings?$select=SomeOpenProperty", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings?$select=SomeOpenProperty"), actualUri);
        }

        [TestMethod]
        public void SelectMixedOpenAndDeclaredPropertiesSucceeds()
        {
            Uri queryUri = new Uri("Paintings?$select=Artist, SomeOpenProperty", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Paintings?$select=" + Uri.EscapeDataString("Artist,SomeOpenProperty"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectingNamespaceQualifiedWildcardsShouldWork()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.*", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=Fully.Qualified.Namespace.*"), actualUri);
        }

        [TestMethod]
        public void ShouldIgnoreCommaAtEndofSelect()
        {
            Uri queryUri = new Uri("People?$select=MyDog,", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=MyDog"), actualUri);
        }
        #endregion $select with no $expand

        #region $expand with no $select

        [TestMethod]
        public void ExpandWithoutSelectShouldDefaultToAllSelections()
        {
            Uri queryUri = new Uri("People?$expand=MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$expand=MyDog"), actualUri);
        }

        [TestMethod]
        public void LastEmbeddedQueryOptionDoesNotRequireSemiColon()
        {
            Uri queryUri = new Uri("People?$expand=MyDog($expand=MyPeople)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($expand=MyPeople)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void BasicNestedExpansionsShouldWork()
        {
            Uri queryUri = new Uri("People?$expand=MyDog($expand=MyPeople($expand=MyPaintings))", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($expand=MyPeople($expand=MyPaintings))"), actualUri.OriginalString);
        }

        [TestMethod]
        public void MultipleExpansionsShouldWork()
        {
            Uri queryUri = new Uri("People?$expand=MyDog, MyPaintings, MyFavoritePainting", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog,MyPaintings,MyFavoritePainting"), actualUri.OriginalString);
        }

        [TestMethod]
        public void MultipleExpandsOnTheSamePropertyAreCollapsed()
        {
            Uri queryUri = new Uri("People?$expand=MyDog, MyDog($expand=MyPeople)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($expand=MyPeople)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ExpandNavigationPropertyOnDerivedType()
        {
            Uri queryUri = new Uri("People?$expand=Fully.Qualified.Namespace.Manager/PaintingsInOffice", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/PaintingsInOffice"), actualUri.OriginalString);
        }

        [TestMethod]
        public void DeepExpandShouldBeMerged()
        {
            Uri queryUri = new Uri("People?$expand=MyDog($expand=MyPeople($expand=MyDog($expand=MyPeople($expand=MyPaintings)))), MyDog($expand=MyPeople($expand=MyDog($expand=MyPeople)))", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($expand=MyPeople($expand=MyDog($expand=MyPeople($expand=MyPaintings))))"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ExpandWithEnumSelect()
        {
            Uri queryUri = new Uri("Dogs?$expand=MyPeople($expand=MyPet2Set($select=PetColorPattern,Color))", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Dogs?$expand=" + Uri.EscapeDataString("MyPeople($expand=MyPet2Set($select=PetColorPattern,Color))"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ParseEnumPropertyOrderByWithinExpand()
        {
            Uri queryUri = new Uri("People?$expand=MyPet2Set($orderby=PetColorPattern desc)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyPet2Set($orderby=PetColorPattern desc)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void RepeatedExpandWithTypeSegmentsShouldBeMerged()
        {
            Uri queryUri = new Uri("People?$expand=Fully.Qualified.Namespace.Manager/DirectReports, Fully.Qualified.Namespace.Manager/DirectReports", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/DirectReports"), actualUri.OriginalString);
        }

        [TestMethod]
        public void DeepExpandWithDifferentTypeSegmentsShouldNotBeMerged()
        {
            Uri queryUri = new Uri("People?$expand=Fully.Qualified.Namespace.Manager/DirectReports,   Fully.Qualified.Namespace.Employee/Manager", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/DirectReports,Fully.Qualified.Namespace.Employee/Manager"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ShouldIgnoreCommaAtEndofExpand()
        {
            Uri queryUri = new Uri("People?$expand=MyDog,", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$expand=MyDog"), actualUri);
        }

        [TestMethod]
        public void ExpandWithInnerQueryOptions()
        {
            Uri queryUri = new Uri("People?$expand=Fully.Qualified.Namespace.Manager/DirectReports($levels=max;$orderby=ID desc),Fully.Qualified.Namespace.Employee/Manager($levels=3)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(
                "http://gobbledygook/People?$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/DirectReports($orderby=ID desc;$levels=max),Fully.Qualified.Namespace.Employee/Manager($levels=3)"),
                actualUri.OriginalString);
        }
        #endregion

        #region Interesting $expand with other options scenarios
        [TestMethod]
        public void NestedSelectPropertyWithJustNavPropAtParentLevelMeansJustOnePropertyAtInnerLevel()
        {
            Uri queryUri = new Uri("Dogs?$select=MyPeople&$expand=MyPeople($select=Name)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Dogs?$select=" + Uri.EscapeDataString("MyPeople") + "&$expand=" + Uri.EscapeDataString("MyPeople($select=Name)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void NestedSelectPropertyWithNothingSelectedAtParentLevelMeansAllAtTopLevelAndJustOnePropertyAtInnerLevel()
        {
            Uri queryUri = new Uri("Dogs?$expand=MyPeople($select=Name)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Dogs?$expand=" + Uri.EscapeDataString("MyPeople($select=Name)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ExpandsDoNotHaveToAppearInSelectToBeSelected()
        {
            Uri queryUri = new Uri("People?$select=MyAddress&$expand=MyDog, MyFavoritePainting", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("MyAddress,MyDog,MyFavoritePainting") + "&$expand=" + Uri.EscapeDataString("MyDog,MyFavoritePainting"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SomeExpandedNavPropsCanAppearInSelectAndAreRetainedAsNavPropLinks()
        {
            Uri queryUri = new Uri("People?$select=MyAddress, MyDog&$expand=MyDog, MyFavoritePainting", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("MyAddress,MyDog,MyFavoritePainting") + "&$expand=" + Uri.EscapeDataString("MyDog,MyFavoritePainting"), actualUri.OriginalString);
        }

        [TestMethod]
        public void MultipleDeepLevelExpansionsAndSelectionsShouldWork()
        {
            Uri queryUri = new Uri("People?$select=MyDog, MyFavoritePainting&$expand=MyDog($expand=MyPeople($select=Name)), MyFavoritePainting($select=Artist)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("MyDog,MyFavoritePainting") + "&$expand=" + Uri.EscapeDataString("MyDog($expand=MyPeople($select=Name)),MyFavoritePainting($select=Artist)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SimpleExpandAndOnlySelectIt()
        {
            Uri queryUri = new Uri("People?$select=MyDog&$expand=MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=MyDog&$expand=MyDog"), actualUri);
        }

        [TestMethod]
        public void ExpandSupportsTypeSegments()
        {
            Uri queryUri = new Uri("People?$expand=Fully.Qualified.Namespace.Employee/PaintingsInOffice", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/PaintingsInOffice"), actualUri.OriginalString);
        }

        [TestMethod]
        public void UnneededTypeSegmentOnSelectButNotExpandIsIgnored()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Employee/MyDog&$expand=MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/MyDog,MyDog") + "&$expand=MyDog", actualUri.OriginalString);
        }

        [TestMethod]
        public void UnneededTypeOnExpandButNotSelectIsKept()
        {
            Uri queryUri = new Uri("People?$select=MyDog&$expand=Fully.Qualified.Namespace.Employee/MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("MyDog,Fully.Qualified.Namespace.Employee/MyDog") + "&$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/MyDog"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectAndExpandWithDifferentTypesWorks()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Employee/MyDog&$expand=Fully.Qualified.Namespace.Employee/MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/MyDog") + "&$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/MyDog"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ExpandSamePropertyOnTwoDifferentTypesWithoutASelectExpandsNavPropOnBothTypes()
        {
            Uri queryUri = new Uri("People?$expand=Fully.Qualified.Namespace.Employee/MyDog, Fully.Qualified.Namespace.Manager/MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Employee/MyDog,Fully.Qualified.Namespace.Manager/MyDog"), actualUri.OriginalString);
        }

        [TestMethod]
        public void WildCardOnExpandedNavigationPropertyAfterTypeSegment()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Manager/MyPaintings&$expand=Fully.Qualified.Namespace.Manager/MyPaintings($select=*)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/MyPaintings") + "&$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/MyPaintings($select=*)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void WildCardOnExpandedNavigationPropertyOnDerivedType()
        {
            Uri queryUri = new Uri("People?$select=Fully.Qualified.Namespace.Manager/PaintingsInOffice&$expand=Fully.Qualified.Namespace.Manager/PaintingsInOffice($select=*)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/PaintingsInOffice") + "&$expand=" + Uri.EscapeDataString("Fully.Qualified.Namespace.Manager/PaintingsInOffice($select=*)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void MixOfSelectionTypesShouldWork()
        {
            Uri queryUri = new Uri("People?$select=Name,Birthdate,MyAddress,Fully.Qualified.Namespace.*,MyLions&$expand=MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,Birthdate,MyAddress,Fully.Qualified.Namespace.*,MyLions,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectingANavPropIsNotRecursiveAllSelection()
        {
            Uri queryUri = new Uri("People?$select=MyDog&$expand=MyDog($expand=MyPeople($select=*))", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($expand=MyPeople($select=*))"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectOnComplexTypeWorks()
        {
            Uri queryUri = new Uri("Paintings?$select=City&$expand=", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Paintings?$select=" + Uri.EscapeDataString("City"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectOnEnumTypeWorks()
        {
            Uri queryUri = new Uri("Pet2Set?$select=PetColorPattern", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Pet2Set?$select=" + Uri.EscapeDataString("PetColorPattern"), actualUri.OriginalString);
        }

        [TestMethod]
        public void MultipleSelectsOnTheSameExpandItem()
        {
            Uri queryUri = new Uri("People?$expand=MyDog($select=Color,Breed)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($select=Color,Breed)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void RedundantExpandsWithUniqueSelectsArePropertyCollapsed()
        {
            Uri queryUri = new Uri("People?$expand=MyDog($select=Color), MyDog($select=Breed)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($select=Breed,Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void TypeSegmentsWorkOnSubExpands()
        {
            Uri queryUri = new Uri("Dogs?$expand=MyPeople($select=Fully.Qualified.Namespace.Employee/Name)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/Dogs?$expand=" + Uri.EscapeDataString("MyPeople($select=Fully.Qualified.Namespace.Employee/Name)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ExplicitNavPropIsAddedIfNeededAtDeeperLevels()
        {
            Uri queryUri = new Uri("People?$expand=MyDog($select=Color;$expand=MyPeople)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($select=Color,MyPeople;$expand=MyPeople)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectAndExpandShouldWorkOnSelectComplexProperties()
        {
            Uri queryUri = new Uri("People?$select=Name,MyAddress/City,MyDog&$expand=MyDog($select=Color)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyAddress/City,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($select=Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectAndExpandShouldWorkOnSelectComplexPropertiesWithTypeCast()
        {
            Uri queryUri = new Uri("People?$select=Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog&$expand=MyDog($select=Color)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($select=Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectAndExpandShouldWorkOnSelectComplexPropertiesWithMultipleTypeCasts()
        {
            Uri queryUri = new Uri("People?$select=Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/NextHome/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog&$expand=MyDog($select=Color)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/NextHome/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($select=Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectAndExpandShouldWorkOnSelectComplexPropertiesRecursively()
        {
            Uri queryUri = new Uri("People?$select=Name,MyAddress/NextHome/NextHome/City,MyDog&$expand=MyDog($select=Color)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyAddress/NextHome/NextHome/City,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($select=Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void SelectAndExpandShouldWorkOnSelectOpenProperty()
        {
            Uri queryUri = new Uri("People?$select=Name,MyOpenAddress/Test,MyDog&$expand=MyDog($select=Color)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyOpenAddress/Test,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($select=Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void TranslateSelectExpandClauseForExpandItemShouldWork()
        {
            string expandClause = "MyDog($filter=Color eq 'Brown';$orderby=Color;$expand=MyPeople/$ref)";
            var topLeveItem = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string> { { "$expand", expandClause }, { "$select","" } }).ParseSelectAndExpand();
            SelectExpandClauseToStringBuilder translater = new SelectExpandClauseToStringBuilder();
            string result = translater.TranslateSelectExpandClause(topLeveItem, false);
            Assert.AreEqual("$expand=" + expandClause, result);
        }

        [TestMethod]
        public void TranslateSelectExpandClauseWithoutExpandRefOptionShouldWork()
        {
            string expandClause = "MyDog($expand=MyPeople/$ref)";
            var topLeveItem = new ODataQueryOptionParser(HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), new Dictionary<string, string> { { "$expand", expandClause }, { "$select", "" } }).ParseSelectAndExpand();
            SelectExpandClauseToStringBuilder translater = new SelectExpandClauseToStringBuilder();
            string result = translater.TranslateSelectExpandClause(topLeveItem, false);
            Assert.AreEqual("$expand=" + expandClause, result);
        }
        #endregion

        #region mixed examples
        [TestMethod]
        public void SelectAllWithoutExpandShouldWork()
        {
            Uri queryUri = new Uri("People?$select=*", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=*"), actualUri);
        }

        [TestMethod]
        public void EmptySelectAndWithoutExpandShouldIgnored()
        {
            Uri queryUri = new Uri("People?$select = ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People"), actualUri);
        }

        [TestMethod]
        public void SelectAllWithEmptyExpandShouldWork()
        {
            Uri queryUri = new Uri("People?$select = *&$expand = ", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=*"), actualUri);
        }

        [TestMethod]
        public void EmptySelectAndEmptyExpandShouldWork()
        {
            Uri queryUri = new Uri("People?$select = &$expand=", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People"), actualUri);
        }

        [TestMethod]
        public void SelectWithEmptyExpandShouldWork()
        {
            Uri queryUri = new Uri("People?$select = FirstName&$expand=", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$select=FirstName"), actualUri);
        }

        [TestMethod]
        public void EmptySelectWithExpandShouldWork()
        {
            Uri queryUri = new Uri("People?$select = &$expand=MyDog", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People?$expand=MyDog"), actualUri);
        }

        [TestMethod]
        public void SelectWithNestedExpandShouldWork()
        {
            Uri queryUri = new Uri("People?$select=Name,MyOpenAddress/Test&$expand=MyDog($filter=Color eq 'Brown';$orderby=Color; $search=Color)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual("http://gobbledygook/People?$select=" + Uri.EscapeDataString("Name,MyOpenAddress/Test,MyDog") + "&$expand=" + Uri.EscapeDataString("MyDog($filter=Color eq 'Brown';$orderby=Color;$search=Color)"), actualUri.OriginalString);
        }

        [TestMethod]
        public void ExpandWithNestedQueryOptionsShouldWork()
        {
            var ervFilter = new EntityRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            var ervOrderby = new EntityRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            var expand =
                new ExpandedNavigationSelectItem(
                    new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)),
                    HardCodedTestModel.GetPeopleSet(),
                    null,
                    new FilterClause(
                        new BinaryOperatorNode(
                            BinaryOperatorKind.Equal,
                            new SingleValuePropertyAccessNode(new EntityRangeVariableReferenceNode("$it", ervFilter), HardCodedTestModel.GetDogColorProp()),
                            new ConstantNode("Brown", "'Brown'")),
                            ervFilter),
                    new OrderByClause(
                        null,
                        new SingleValuePropertyAccessNode(new EntityRangeVariableReferenceNode("$it", ervOrderby), HardCodedTestModel.GetDogColorProp()),
                        OrderByDirection.Ascending,
                        ervOrderby),
                    1,
                    /* skipOption */ null,
                    true,
                    new SearchClause(new SearchTermNode("termX")),
                    /* levelsOption*/ null);

            ODataUri uri = new ODataUri()
            {
                ServiceRoot = new Uri("http://gobbledygook/"),
                Path = new ODataPath(new EntitySetSegment(HardCodedTestModel.GetPeopleSet())),
                SelectAndExpand = new SelectExpandClause(new[] { expand }, true)
            };

            Uri actualUri = new ODataUriBuilder(ODataUrlConventions.Default, uri).BuildUri();
            Assert.AreEqual("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyDog($filter=Color eq 'Brown';$orderby=Color;$top=1;$count=true;$search=termX)"), actualUri.OriginalString);
        }
        #endregion

        public static Uri UriBuilder(Uri queryUri, ODataUrlConventions urlConventions, ODataUriParserSettings settings)
        {
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, queryUri);
            odataUriParser.UrlConventions = urlConventions;
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(urlConventions, odataUri);
            return odataUriBuilder.BuildUri();
        }
    }
}
