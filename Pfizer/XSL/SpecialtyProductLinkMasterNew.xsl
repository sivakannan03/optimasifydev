<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:include href="PagingTemplate.xsl" />
  <xsl:template match="/">
    <div id="tableContainer">
      <table align="left" width="50%" cellspacing="1" id="tblBrand" class="inner_tb"  border="1" cellpadding="0" boreder="1">
        <tr class="tableContent1" align="center">
          <th class="tableheader">Product</th>
          <th class="tableheader">DR Flag</th>
          <th class="tableheader">Delete</th>
        </tr>
        <xsl:choose>
          <xsl:when test="count(//row[@Element = 'Prod']/@ProductFKID) &gt;0">
            <xsl:apply-templates select="//row[@Element = 'Prod']" />
          </xsl:when>
          <xsl:otherwise>
            <tr>
              <td align="center" class="tb-r1" colspan="2">No Records Found</td>
            </tr>
          </xsl:otherwise>
        </xsl:choose>
      </table>
    </div>
  </xsl:template>
  <xsl:template match="//row[@Element = 'Prod']">
    <xsl:variable name="ProdFKID" select="@ProductFKID" />
    <xsl:variable name="DrFlag" select="@DRFlag" />
    <tr class="tablecontent2">
      <td class="tb-r1" align="center">
        <select parameter="ProductFKID" class="largedropdownbox" valid="m" id="drpProductFKID1" errname="Product">
          <option>
            <xsl:attribute name="value"></xsl:attribute>Select
          </option>
          <xsl:for-each select="//row[@Element = 'Product']">
            <option>
              <xsl:attribute name="value">
                <xsl:value-of select="@ProductFKID" />
              </xsl:attribute>
              <xsl:if test="$ProdFKID = @ProductFKID">
                <xsl:attribute name="selected"></xsl:attribute>
              </xsl:if>
              <xsl:value-of select="@ProductName" />
            </option>
          </xsl:for-each>
        </select>
      </td>
      <td class="tb-r1" align="center">
        <select parameter="ProductFKID" class="smalldropdownboxRR" valid="m" id="drpDRFlag1" name="drpDRFlag1" errname="Product" style="width:80px;">
          <option>
            <xsl:attribute name="value"></xsl:attribute>Select
          </option>
          <option>
            <xsl:attribute name="value">D</xsl:attribute>
            <xsl:attribute name="selected"></xsl:attribute>
            <xsl:if test="$DrFlag = 'D'">
              <xsl:attribute name="selected"></xsl:attribute>
            </xsl:if>
            D
          </option>
          <option>
            <xsl:attribute name="value">R</xsl:attribute>
            <xsl:if test="$DrFlag = 'R'">
              <xsl:attribute name="selected"></xsl:attribute>
            </xsl:if>
            R
          </option>
        </select>
      </td>
      <td class="tb-r1" align="center">
        <input type="checkbox" class="checkbox" />
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>