<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
				xmlns:msxsl="urn:schemas-microsoft-com:xslt"
version="2.0">
				

	<msxsl:script language="javascript" implements-prefix="msxsl">
	   		function printPagingInformation(totalRecords, pageSize) {
					var totalPage = parseInt(totalRecords/pageSize);
					if( (totalRecords % pageSize) > 0) totalPage++;
					
					return totalPage;
				}
			
	</msxsl:script>
			  
	
	<xsl:template name="paging-template">
		<xsl:variable name="total-records">
			<xsl:value-of select="//row[@TotalRecords]/@TotalRecords"/>
		</xsl:variable>
		
		<xsl:variable name="page-no">
			<xsl:value-of select="//row[@TotalRecords]/@PageNo"/>
		</xsl:variable>
		
		<xsl:variable name="page-size">
			<xsl:value-of select="//row[@TotalRecords]/@PageSize"/>
		</xsl:variable>
		
		<xsl:variable name="total-page">
			<xsl:value-of select="msxsl:printPagingInformation($total-records, $page-size)"/>
		</xsl:variable>
		
		<table width="100%" border="1" cellpadding="0" cellspacing="0"   align="center">
			<tr>
				<td class='text1bold' >
				
						<!--First Page-->
							<xsl:if test="$page-no &gt; 1">
						<a >
							<xsl:attribute name="href">javascript:pageNumberClicked(<xsl:value-of select="number(1)"/>)</xsl:attribute>
							<img src="../../images/first_1.gif" alt="First" border="0" />
						</a>
						&#160;
					</xsl:if>
						
						<!--First Page-->
					<xsl:if test="$page-no &gt; 1">
					<IMG src="../../images/prev_1.gif" alt="Previous">
						<xsl:attribute name="onclick">javascript:pageNumberClicked(<xsl:value-of select="number($page-no)-1"/>)</xsl:attribute>
					</IMG>
						
					</xsl:if>
					<!--First Page-->
&#160;&#160;&#160;&#160;&#160;
					<xsl:if test="$page-no &lt; $total-page">
					
						<IMG src="../../images/next_1.gif" alt="Next">
						<xsl:attribute name="onclick">javascript:pageNumberClicked(<xsl:value-of select="number($page-no)+1"/>)</xsl:attribute>
					</IMG>
						
					</xsl:if>
					
					<!-- LastPage-->
					<xsl:if test="$page-no &lt; $total-page">
						<a class="link1">
							<xsl:attribute name="href">javascript:pageNumberClicked(<xsl:value-of select="number($total-page)"/>)</xsl:attribute>
							<img src="../../images/last_1.gif" alt="Last"  border="0"/>
							
						</a>&#160;
					</xsl:if>
					<!-- LastPage-->
				</td>
			
				<td align="right" class='text1bold'>
		
					Page 
						<input type="text" name="txtCurrentPageNo" id="txtCurrentPageNo" class='textbox' size='3'>
							<xsl:attribute name="value">
								<xsl:value-of select="$page-no"/>
							</xsl:attribute>
						</input>
					of
						<xsl:value-of select="msxsl:printPagingInformation($total-records, $page-size)"/>
						<input type='button' class='button' onClick='return searchResult()' value="Go"/>
						<input type="hidden" name="gHidTotalPage">
										
						
							<xsl:attribute name="value">
								<xsl:value-of select="$total-page"/>
							</xsl:attribute>
						</input>
				</td>
			</tr>
		</table>
	</xsl:template>
	
</xsl:stylesheet>	